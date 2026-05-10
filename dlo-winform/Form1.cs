using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace dlo_winform;

public partial class Form1 : Form
{
    private const int AnimationTickMilliseconds = 500;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GraphData GD = null!;
    private System.Windows.Forms.Timer animationTimer = null!;
    private DijkstraRouteResult currentRoute;
    private PacketSimulation currentSimulation;
    private bool isPaused = false;
    private bool isAddNodeMode = false;
    private bool isRemoveNodeMode = false;
    private NetworkEdge editingEdge = null;
    private double lastCalcMs;
    private bool loadingSampleList = true;
    private readonly Font defaultFont, boldFont;
    private readonly Pen edgePen, pathPen;
    private ToolTip graphToolTip;
    private object lastHoveredObject = null;
    private float zoom = 1.0f;
    private float panX = 0f;
    private float panY = 0f;
    private bool isPanning = false;
    private Point lastMousePos;
    private bool isGraphDirty = true;
    private NetworkNode[] sortedNodesX;
    private NetworkEdge[] sortedEdgesX;
    private float maxEdgeDx = 0f;

    public Form1()
    {
        InitializeComponent();
        defaultFont = SystemFonts.DefaultFont;
        boldFont = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size, FontStyle.Bold);
        edgePen = new Pen(Color.Black, 2);
        pathPen = new Pen(Color.DodgerBlue, 3);
        pbxCanvas.Paint += pbxCanvas_Paint;
        pbxCanvas.MouseDown += pbxCanvas_MouseDown;
        pbxCanvas.MouseMove += pbxCanvas_MouseMove;
        pbxCanvas.MouseUp += pbxCanvas_MouseUp;
        pbxCanvas.MouseDoubleClick += pbxCanvas_MouseDoubleClick;
        pbxCanvas.MouseWheel += pbxCanvas_MouseWheel;
        pbxCanvas.MouseEnter += pbxCanvas_MouseEnter;
        btnGenerate.Click += btnGenerate_Click;
        graphToolTip = new ToolTip();
        graphToolTip.AutoPopDelay = 5000;
        graphToolTip.InitialDelay = 200;
        graphToolTip.ReshowDelay = 100;
        if (!DesignMode)
        {
            this.DoubleBuffered = true;
            GD = SampleGraphs.CreateAt(0, pbxCanvas.Width, pbxCanvas.Height);
            isGraphDirty = true;
            cmbSampleGraphs.Items.AddRange(SampleGraphs.Names.ToArray());
            cmbSampleGraphs.SelectedIndex = 0;
            loadingSampleList = false;
            cmbSampleGraphs.SelectedIndexChanged += cmbSampleGraphs_SelectedIndexChanged;
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = AnimationTickMilliseconds;
            animationTimer.Tick += AnimationTimer_Tick;
        }
    }
    private void UpdateSortedCache()
    {
        if (!isGraphDirty || GD == null) return;

        sortedNodesX = GD.nodeList.OrderBy(n => n.Position.X).ToArray();
        sortedEdgesX = GD.edgeList.OrderBy(e => Math.Min(e.StartNode.Position.X, e.EndNode.Position.X)).ToArray();

        maxEdgeDx = 0f;
        foreach (var e in sortedEdgesX)
        {
            float dx = Math.Abs(e.StartNode.Position.X - e.EndNode.Position.X);
            if (dx > maxEdgeDx) maxEdgeDx = dx;
        }
        isGraphDirty = false;
    }

    private int GetFirstVisibleNodeIndex(NetworkNode[] nodes, float targetX)
    {
        if (nodes == null || nodes.Length == 0) return 0;
        int low = 0, high = nodes.Length - 1, ans = nodes.Length;
        while (low <= high)
        {
            int mid = low + (high - low) / 2;
            if (nodes[mid].Position.X >= targetX)
            {
                ans = mid;
                high = mid - 1;
            }
            else low = mid + 1;
        }
        return ans;
    }

    private int GetFirstVisibleEdgeIndex(NetworkEdge[] edges, float minXThreshold)
    {
        if (edges == null || edges.Length == 0) return 0;
        int low = 0, high = edges.Length - 1, ans = edges.Length;
        while (low <= high)
        {
            int mid = low + (high - low) / 2;
            float minX = Math.Min(edges[mid].StartNode.Position.X, edges[mid].EndNode.Position.X);
            if (minX >= minXThreshold)
            {
                ans = mid;
                high = mid - 1;
            }
            else low = mid + 1;
        }
        return ans;
    }

    private NetworkEdge TryGetEdgeNearWorldPoint(PointF worldPos, float tolerance)
    {
        if (sortedEdgesX == null || sortedEdgesX.Length == 0) return null;
        int startIndex = GetFirstVisibleEdgeIndex(sortedEdgesX, worldPos.X - maxEdgeDx - tolerance);

        for (int i = startIndex; i < sortedEdgesX.Length; i++)
        {
            NetworkEdge edge = sortedEdgesX[i];
            float minX = Math.Min(edge.StartNode.Position.X, edge.EndNode.Position.X);
            if (minX > worldPos.X + tolerance) break;

            float minY = Math.Min(edge.StartNode.Position.Y, edge.EndNode.Position.Y);
            float maxY = Math.Max(edge.StartNode.Position.Y, edge.EndNode.Position.Y);
            if (worldPos.Y < minY - tolerance || worldPos.Y > maxY + tolerance) continue;

            float dist = GeometryHelpers.DistanceToSegment(worldPos, edge.StartNode.Position, edge.EndNode.Position);
            if (dist < tolerance) return edge;
        }
        return null;
    }

    private NetworkNode GetNodeWithZoomTolerance(PointF worldPos)
    {
        if (sortedNodesX == null || sortedNodesX.Length == 0) return null;
        float screenHitRadius = 15f;
        float worldHitRadius = screenHitRadius / zoom;

        float searchX = worldPos.X - worldHitRadius - 5f;
        int startIndex = GetFirstVisibleNodeIndex(sortedNodesX, searchX);

        for (int i = startIndex; i < sortedNodesX.Length; i++)
        {
            NetworkNode node = sortedNodesX[i];
            if (node.Position.X > worldPos.X + worldHitRadius) break;

            float dx = worldPos.X - node.Position.X;
            float dy = worldPos.Y - node.Position.Y;
            float distSq = dx * dx + dy * dy;

            if (distSq <= worldHitRadius * worldHitRadius) return node;
        }
        return null;
    }

    private void ResetView()
    {
        zoom = 1.0f;
        panX = 0f;
        panY = 0f;
    }

    private PointF ScreenToWorld(Point screenPos)
    {
        return new PointF((screenPos.X - panX) / zoom, (screenPos.Y - panY) / zoom);
    }

    private void pbxCanvas_MouseEnter(object sender, EventArgs e)
    {
        if (!pbxCanvas.Focused) pbxCanvas.Focus();
    }

    private void pbxCanvas_MouseWheel(object sender, MouseEventArgs e)
    {
        float oldZoom = zoom;
        if (e.Delta > 0) zoom *= 1.15f;
        else if (e.Delta < 0) zoom /= 1.15f;

        if (zoom < 0.05f) zoom = 0.05f;
        if (zoom > 50f) zoom = 50f;

        float worldX = (e.X - panX) / oldZoom;
        float worldY = (e.Y - panY) / oldZoom;

        panX = e.X - worldX * zoom;
        panY = e.Y - worldY * zoom;

        pbxCanvas.Invalidate();
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtGenNodes.Text, out int n) || n <= 0)
        {
            MessageBox.Show("Please enter a valid positive nodes number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (!int.TryParse(txtGenEdges.Text, out int m) || m < 0)
        {
            MessageBox.Show("Please enter a valid positive edges number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        long maxEdges = 1L * n * (n - 1) / 2;
        if (m > maxEdges) m = (int)maxEdges;

        ResetView();

        GD = SampleGraphs.GenerateMassiveGraph(n, m, pbxCanvas.Width, pbxCanvas.Height);
        foreach (NetworkEdge edge in GD.edgeList)
        {
            if (edge.TransferSpeedBytesPerSecond == 0)
                edge.TransferSpeedBytesPerSecond = GraphEditor.GenerateTransferSpeed();
        }

        isGraphDirty = true;
        currentRoute = null;
        currentSimulation = null;
        animationTimer.Stop();
        txtLog.Clear();

        pbxCanvas.Invalidate();
    }

    private void cmbSampleGraphs_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (loadingSampleList) return;
        LoadSampleGraph(cmbSampleGraphs.SelectedIndex);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (GD.nodeList.Count <= 0 || GD.edgeList.Count <= 0)
        {
            MessageBox.Show("Graph data cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        long packetBytes = ParsePacketSize();
        if (packetBytes <= 0)
        {
            MessageBox.Show("Please enter a valid positive packet size.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!int.TryParse(txtStartNode.Text, out int startId) || !int.TryParse(txtDestNode.Text, out int destId))
        {
            MessageBox.Show("Please enter valid integer values for start and destination nodes.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        NetworkNode startNode = GD.GetNodeById(startId), destNode = GD.GetNodeById(destId);

        if (startNode == null || destNode == null)
        {
            MessageBox.Show("Start or destination node not found in graph.", "Node Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        GraphEditor.RecalculateEdgeWeights(GD, packetBytes);

        Stopwatch sw = Stopwatch.StartNew();
        currentRoute = DijkstraGraphService.FindRoute(GD, startId, destId);
        sw.Stop();
        lastCalcMs = sw.Elapsed.TotalMilliseconds;

        if (!currentRoute.Reachable)
        {
            Log("Route not found from node " + startId + " to node " + destId);
            MessageBox.Show("No route found between the specified nodes.", "Unreachable", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        currentSimulation = new PacketSimulation(currentRoute);
        isPaused = false;
        txtLog.Clear();
        Log("Route found: " + string.Join(" -> ", currentRoute.PathNodeIds));

        animationTimer.Start();
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        if (currentSimulation == null || isPaused) return;

        PacketTickResult tick = currentSimulation.Tick();

        if (tick.IsComplete)
        {
            Log("Packet delivered to node " + currentRoute?.DestinationNodeId + " in " + tick.TotalElapsedTime + " ms, calculation time: " + lastCalcMs.ToString("F3") + " ms");
            animationTimer.Stop();
        }
        else if (tick.IsMove && currentRoute != null && currentSimulation.CurrentEdgeIndex < currentRoute.PathEdges.Count)
        {
            NetworkEdge edge = currentRoute.PathEdges[currentSimulation.CurrentEdgeIndex];
            string speedStr = GraphEditor.SpeedToMbpsString(edge.TransferSpeedBytesPerSecond);
            Log(tick.FromNodeId + " -> " + tick.ToNodeId + ", speed: " + speedStr + ", transfer time: " + tick.TickTravelTime + " ms");
        }

        pbxCanvas.Invalidate();
    }

    private void button2_Click(object sender, EventArgs e)
    {
        animationTimer.Stop();
        currentSimulation = null;
        currentRoute = null;
        pbxCanvas.Invalidate();
    }

    private void button4_Click(object sender, EventArgs e)
    {
        isPaused = !isPaused;
        button4.Text = isPaused ? "Resume" : "Pause";
    }

    private void pbxCanvas_Paint(object sender, PaintEventArgs e)
    {
        if (GD == null) return;
        UpdateSortedCache();

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        float wLeft = -panX / zoom - 50;
        float wRight = (pbxCanvas.Width - panX) / zoom + 50;
        float wTop = -panY / zoom - 50;
        float wBottom = (pbxCanvas.Height - panY) / zoom + 50;

        int firstNodeIdx = GetFirstVisibleNodeIndex(sortedNodesX, wLeft);
        int lastNodeIdx = GetFirstVisibleNodeIndex(sortedNodesX, wRight);
        if (lastNodeIdx < sortedNodesX.Length && sortedNodesX[lastNodeIdx].Position.X < wRight) lastNodeIdx++;

        int firstEdgeIdx = GetFirstVisibleEdgeIndex(sortedEdgesX, wLeft - maxEdgeDx);
        int lastEdgeIdx = GetFirstVisibleEdgeIndex(sortedEdgesX, wRight);
        if (lastEdgeIdx < sortedEdgesX.Length) lastEdgeIdx++;

        bool isMassiveGraph = GD.nodeList.Count >= 1000 || GD.edgeList.Count >= 2000;
        bool drawDetails = !isMassiveGraph || zoom >= 3.0f;

        int edgesInView = lastEdgeIdx - firstEdgeIdx;
        int edgeStep = (isMassiveGraph && !drawDetails && edgesInView > 8000) ? edgesInView / 8000 : 1;

        int nodesInView = lastNodeIdx - firstNodeIdx;
        int nodeStep = (isMassiveGraph && !drawDetails && nodesInView > 10000) ? nodesInView / 10000 : 1;

        if (isPanning)
        {
            edgeStep *= 2;
            nodeStep *= 2;
        }

        if (edgeStep < 1) edgeStep = 1;
        if (nodeStep < 1) nodeStep = 1;
        float fixedNodeRadius = 11f;
        float fixedSmallDotSize = 4f;
        float fixedPacketRadius = 6f;

        using (Pen thinEdgePen = new Pen(Color.LightGray, 1f))
        using (Pen mainEdgePen = new Pen(Color.Black, 2f))
        using (Pen nodeBorderPen = new Pen(Color.DarkBlue, 2f))
        using (Pen routePathPen = new Pen(Color.DodgerBlue, 4f))
        {
            int drawnEdges = 0;
            for (int i = firstEdgeIdx; i < lastEdgeIdx; i += edgeStep)
            {
                if (i >= sortedEdgesX.Length) break;
                NetworkEdge edge = sortedEdgesX[i];

                float maxX = Math.Max(edge.StartNode.Position.X, edge.EndNode.Position.X);
                if (maxX < wLeft) continue;

                float minY = Math.Min(edge.StartNode.Position.Y, edge.EndNode.Position.Y);
                float maxY = Math.Max(edge.StartNode.Position.Y, edge.EndNode.Position.Y);
                if (maxY < wTop || minY > wBottom) continue;

                float sx1 = edge.StartNode.Position.X * zoom + panX;
                float sy1 = edge.StartNode.Position.Y * zoom + panY;
                float sx2 = edge.EndNode.Position.X * zoom + panX;
                float sy2 = edge.EndNode.Position.Y * zoom + panY;

                if (drawDetails)
                {
                    g.DrawLine(mainEdgePen, sx1, sy1, sx2, sy2);
                    float midX = (sx1 + sx2) / 2f;
                    float midY = (sy1 + sy2) / 2f;
                    string speedText = GraphEditor.SpeedToMbpsString(edge.TransferSpeedBytesPerSecond);
                    g.DrawString(speedText, defaultFont, Brushes.Black, midX, midY);
                }
                else
                {
                    g.DrawLine(thinEdgePen, sx1, sy1, sx2, sy2);
                }

                drawnEdges++;
                if (drawnEdges > 8000) break;
            }

            if (currentRoute != null)
            {
                foreach (NetworkEdge edge in currentRoute.PathEdges)
                {
                    float sx1 = edge.StartNode.Position.X * zoom + panX;
                    float sy1 = edge.StartNode.Position.Y * zoom + panY;
                    float sx2 = edge.EndNode.Position.X * zoom + panX;
                    float sy2 = edge.EndNode.Position.Y * zoom + panY;
                    g.DrawLine(routePathPen, sx1, sy1, sx2, sy2);
                }
            }

            if (currentSimulation != null && currentRoute != null && currentSimulation.CurrentEdgeIndex >= 0 && currentSimulation.CurrentEdgeIndex < currentRoute.PathEdges.Count)
            {
                NetworkEdge edge = currentRoute.PathEdges[currentSimulation.CurrentEdgeIndex];
                float packetWorldX = (edge.StartNode.Position.X + edge.EndNode.Position.X) / 2f;
                float packetWorldY = (edge.StartNode.Position.Y + edge.EndNode.Position.Y) / 2f;

                float sx = packetWorldX * zoom + panX;
                float sy = packetWorldY * zoom + panY;

                g.FillEllipse(Brushes.Red, sx - fixedPacketRadius, sy - fixedPacketRadius, fixedPacketRadius * 2, fixedPacketRadius * 2);
            }

            int drawnNodes = 0;
            for (int i = firstNodeIdx; i < lastNodeIdx; i += nodeStep)
            {
                if (i >= sortedNodesX.Length) break;
                NetworkNode node = sortedNodesX[i];
                if (node.Position.Y < wTop || node.Position.Y > wBottom) continue;

                float sx = node.Position.X * zoom + panX;
                float sy = node.Position.Y * zoom + panY;

                Brush nodeBrush = Brushes.Blue;
                if (currentRoute != null && node.Id == currentRoute.StartNodeId) nodeBrush = Brushes.Green;
                else if (currentRoute != null && node.Id == currentRoute.DestinationNodeId) nodeBrush = Brushes.Orange;

                if (drawDetails)
                {
                    if (nodeBrush == Brushes.Blue) nodeBrush = Brushes.LightBlue;

                    float diam = fixedNodeRadius * 2;
                    g.FillEllipse(nodeBrush, sx - fixedNodeRadius, sy - fixedNodeRadius, diam, diam);
                    g.DrawEllipse(nodeBorderPen, sx - fixedNodeRadius, sy - fixedNodeRadius, diam, diam);

                    string idText = node.Id.ToString();
                    SizeF textSize = g.MeasureString(idText, boldFont);
                    g.DrawString(idText, boldFont, Brushes.Black, sx - textSize.Width / 2f, sy - textSize.Height / 2f);
                }
                else
                {
                    float halfDot = fixedSmallDotSize / 2f;
                    g.FillRectangle(nodeBrush, sx - halfDot, sy - halfDot, fixedSmallDotSize, fixedSmallDotSize);
                }

                drawnNodes++;
                if (drawnNodes > 10000) break;
            }
        }
    }

    private void ShowNodeConnectionsPopup(NetworkNode node, Point location)
    {
        List<string> connections = new List<string>();
        foreach (var edge in GD.edgeList)
        {
            if (edge.StartNode == node) connections.Add($"-> Node {edge.EndNode.Id} (Weight: {edge.Weight})");
            else if (edge.EndNode == node) connections.Add($"<- Node {edge.StartNode.Id} (Weight: {edge.Weight})");
        }

        if (connections.Count == 0) return;

        Form popup = new Form
        {
            Text = $"Node {node.Id} ({connections.Count} connections)",
            Size = new Size(250, Math.Min(300, 50 + connections.Count * 20)),
            StartPosition = FormStartPosition.Manual,
            FormBorderStyle = FormBorderStyle.SizableToolWindow,
            TopMost = true,
            ShowInTaskbar = false
        };
        Point screenPos = pbxCanvas.PointToScreen(location);
        Rectangle screen = Screen.FromControl(this).WorkingArea;

        int posX = screenPos.X + 15;
        int posY = screenPos.Y + 15;

        if (posX + popup.Width > screen.Right) posX = screenPos.X - popup.Width - 5;
        if (posY + popup.Height > screen.Bottom) posY = screenPos.Y - popup.Height - 5;

        popup.Location = new Point(posX, posY);

        ListBox listBox = new ListBox
        {
            Dock = DockStyle.Fill,
            IntegralHeight = false,
            Font = new Font("Consolas", 10)
        };

        foreach (var conn in connections) listBox.Items.Add(conn);

        popup.Controls.Add(listBox);
        popup.Deactivate += (s, ev) => popup.Close();

        popup.Show(this);
    }

    private void pbxCanvas_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            isPanning = true;
            lastMousePos = e.Location;
            pbxCanvas.Cursor = Cursors.SizeAll;
            return;
        }

        if (e.Button != MouseButtons.Left) return;
        PointF worldLoc = ScreenToWorld(e.Location);

        if (isAddNodeMode)
        {
            NetworkNode node = GraphEditor.AddNode(GD, worldLoc);
            isGraphDirty = true;
            pbxCanvas.Invalidate();
            Log("Added node " + node.Id + " at (" + worldLoc.X + ", " + worldLoc.Y + ")");
            isAddNodeMode = false;
            return;
        }

        if (isRemoveNodeMode)
        {
            NetworkNode nodeToRemove = GetNodeWithZoomTolerance(worldLoc);
            if (nodeToRemove != null)
            {
                (bool nodeRemoved, int removedCount, List<NetworkEdge> removedEdges) = GraphEditor.RemoveNode(GD, nodeToRemove.Position);
                if (nodeRemoved)
                {
                    isGraphDirty = true;
                    foreach (NetworkEdge edge in removedEdges)
                    {
                        string speedStr = GraphEditor.SpeedToMbpsString(edge.TransferSpeedBytesPerSecond);
                        Log("Edge removed: " + edge.StartNode.Id + " <-> " + edge.EndNode.Id + ", speed: " + speedStr);
                    }
                    Log("Removed node with " + removedCount + " edge(s)");
                    currentRoute = null;
                    currentSimulation = null;
                    pbxCanvas.Invalidate();
                }
            }
            isRemoveNodeMode = false;
            return;
        }

        if (checkBoxEditWeight.Checked)
        {
            NetworkEdge edge = TryGetEdgeNearWorldPoint(worldLoc, 6f / zoom);
            if (edge != null)
            {
                int? newWeight = PromptForWeight(edge.Weight);
                if (newWeight.HasValue)
                {
                    edge.Weight = newWeight.Value;
                    Log("Changed edge weight to " + newWeight.Value);
                    pbxCanvas.Invalidate();
                }
            }
            return;
        }

        if (checkBox1.Checked)
        {
            NetworkNode node = GetNodeWithZoomTolerance(worldLoc);
            if (node != null)
                pbxCanvas.Tag = node;
        }
        if (!isAddNodeMode && !isRemoveNodeMode && !checkBoxEditWeight.Checked && !checkBox1.Checked)
        {
            NetworkNode node = GetNodeWithZoomTolerance(worldLoc);
            if (node != null)
            {
                ShowNodeConnectionsPopup(node, e.Location);
            }
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
    }

    private void pbxCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (isPanning)
        {
            if (e.Button != MouseButtons.Right)
            {
                isPanning = false;
                pbxCanvas.Cursor = Cursors.Default;
                pbxCanvas.Invalidate();
            }
            else
            {
                float dx = e.X - lastMousePos.X;
                float dy = e.Y - lastMousePos.Y;
                panX += dx;
                panY += dy;
                lastMousePos = e.Location;
                pbxCanvas.Invalidate();
                return;
            }
        }

        if (GD == null) return;
        UpdateSortedCache();

        PointF worldLoc = ScreenToWorld(e.Location);

        NetworkNode hoveredNode = GetNodeWithZoomTolerance(worldLoc);
        if (hoveredNode != null)
        {
            if (lastHoveredObject != hoveredNode)
            {
                lastHoveredObject = hoveredNode;
                List<string> connections = new List<string>();
                foreach (var edge in GD.edgeList)
                {
                    if (edge.StartNode == hoveredNode)
                        connections.Add($"-> Node {edge.EndNode.Id} (Weight: {edge.Weight})");
                    else if (edge.EndNode == hoveredNode)
                        connections.Add($"<- Node {edge.StartNode.Id} (Weight: {edge.Weight})");
                }

                string info = $"[Node {hoveredNode.Id}]\nConnection:\n";
                if (connections.Count == 0) info += "No Connection.";
                else
                {
                    int maxDisplay = 15;
                    info += string.Join("\n", connections.Take(maxDisplay));
                    if (connections.Count > maxDisplay)
                        info += $"\n... and {connections.Count - maxDisplay} others connection.";
                }

                graphToolTip.SetToolTip(pbxCanvas, info);
            }
            return;
        }

        NetworkEdge hoveredEdge = TryGetEdgeNearWorldPoint(worldLoc, 5.0f / zoom);
        if (hoveredEdge != null)
        {
            if (lastHoveredObject != hoveredEdge)
            {
                lastHoveredObject = hoveredEdge;
                string speedStr = GraphEditor.SpeedToMbpsString(hoveredEdge.TransferSpeedBytesPerSecond);
                string info = $"[Edge: Node {hoveredEdge.StartNode.Id} -> Node {hoveredEdge.EndNode.Id}]\n" +
                              $"Weight: {hoveredEdge.Weight}\n" +
                              $"Speed: {speedStr}";
                graphToolTip.SetToolTip(pbxCanvas, info);
            }
            return;
        }

        if (lastHoveredObject != null)
        {
            lastHoveredObject = null;
            graphToolTip.SetToolTip(pbxCanvas, "");
        }
    }

    private void pbxCanvas_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            isPanning = false;
            pbxCanvas.Cursor = Cursors.Default;
            pbxCanvas.Invalidate();
            return;
        }

        if (checkBox1.Checked && pbxCanvas.Tag is NetworkNode startNode && e.Button == MouseButtons.Left)
        {
            PointF worldLoc = ScreenToWorld(e.Location);
            var endNode = GetNodeWithZoomTolerance(worldLoc);
            if (endNode != null && startNode != endNode)
            {
                ToggleEdgeOutcome outcome = GraphEditor.ToggleEdge(GD, startNode, endNode);
                isGraphDirty = true;
                NetworkEdge edge = GD.edgeList.FirstOrDefault(ed =>
                    (ed.StartNode == startNode && ed.EndNode == endNode) ||
                    (ed.StartNode == endNode && ed.EndNode == startNode));
                if (outcome == ToggleEdgeOutcome.Created && edge != null)
                {
                    string speedStr = GraphEditor.SpeedToMbpsString(edge.TransferSpeedBytesPerSecond);
                    Log("Edge created: " + startNode.Id + " <-> " + endNode.Id + ", speed: " + speedStr);
                }
                else if (outcome == ToggleEdgeOutcome.Removed)
                {
                    Log("Edge removed: " + startNode.Id + " <-> " + endNode.Id);
                }
                pbxCanvas.Invalidate();
            }
            pbxCanvas.Tag = null;
        }
    }

    private void button3_Click(object sender, EventArgs e)
    {
        checkBoxEditWeight.Checked = false;
        checkBox1.Checked = false;
        isRemoveNodeMode = false;
        isAddNodeMode = true;
        pbxCanvas.Tag = null;
        Log("Click on the canvas to place the new node.");
    }

    private void buttonRemoveNode_Click(object sender, EventArgs e)
    {
        checkBoxEditWeight.Checked = false;
        checkBox1.Checked = false;
        isAddNodeMode = false;
        isRemoveNodeMode = true;
        pbxCanvas.Tag = null;
        Log("Click on the canvas to remove a node.");
    }

    private void checkBoxEditWeight_CheckedChanged(object sender, EventArgs e)
    {
        if (checkBoxEditWeight.Checked && checkBox1.Checked)
            checkBox1.Checked = false;
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (checkBox1.Checked && checkBoxEditWeight.Checked)
            checkBoxEditWeight.Checked = false;
    }

    private void buttonPrev_Click(object sender, EventArgs e)
    {
        int prev = SampleGraphs.PreviousIndex(cmbSampleGraphs.SelectedIndex);
        cmbSampleGraphs.SelectedIndex = prev;
    }

    private void button5_Click(object sender, EventArgs e)
    {
        int next = SampleGraphs.NextIndex(cmbSampleGraphs.SelectedIndex);
        cmbSampleGraphs.SelectedIndex = next;
    }

    private void LoadSampleGraph(int index)
    {
        ResetView();
        GD = SampleGraphs.CreateAt(index, pbxCanvas.Width, pbxCanvas.Height);
        foreach (NetworkEdge edge in GD.edgeList)
        {
            if (edge.TransferSpeedBytesPerSecond == 0)
                edge.TransferSpeedBytesPerSecond = GraphEditor.GenerateTransferSpeed();
        }

        isGraphDirty = true;
        currentRoute = null;
        currentSimulation = null;
        animationTimer.Stop();
        txtLog.Clear();
        Log($"Loaded sample graph: {cmbSampleGraphs.Text}. Right Click to Pan, Scroll to Zoom.");
        pbxCanvas.Invalidate();
    }

    private static int? PromptForWeight(int currentWeight)
    {
        using var form = new Form();
        form.Text = "Edit Edge Weight";
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.ClientSize = new Size(260, 150);
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowInTaskbar = false;

        Label label = new Label { Text = "New weight:", Location = new Point(12, 20), Size = new Size(80, 25) };
        TextBox textBox = new TextBox { Location = new Point(100, 20), Size = new Size(140, 27), Text = currentWeight.ToString() };
        Button ok = new Button { Text = "OK", Location = new Point(50, 70), Size = new Size(75, 30), DialogResult = DialogResult.OK };
        Button cancel = new Button { Text = "Cancel", Location = new Point(135, 70), Size = new Size(75, 30), DialogResult = DialogResult.Cancel };

        form.Controls.Add(label);
        form.Controls.Add(textBox);
        form.Controls.Add(ok);
        form.Controls.Add(cancel);
        form.AcceptButton = ok;
        form.CancelButton = cancel;

        if (form.ShowDialog() == DialogResult.OK && int.TryParse(textBox.Text, out int weight) && weight >= 0)
            return weight;
        return null;
    }

    private long ParsePacketSize()
    {
        if (!int.TryParse(txtPacketSize.Text, out int value) || value <= 0)
            return -1;
        var unit = (PacketUnit)cmbPacketUnit.SelectedIndex;
        return GraphEditor.PacketSizeBytes(value, unit);
    }

    private void Log(string message)
    {
        txtLog.AppendText(message + Environment.NewLine);
    }

    private void groupBox1_Enter(object sender, EventArgs e) { }
    private void groupBox2_Enter(object sender, EventArgs e) { }
    private void groupBox3_Enter(object sender, EventArgs e) { }
    private void Form1_Load(object sender, EventArgs e) { }

    private void pbxCanvas_MouseDoubleClick(object sender, MouseEventArgs e) { }

    private void txtEdgeWeightEditor_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            txtEdgeWeightEditor_LostFocus(sender, EventArgs.Empty);
            e.Handled = true;
        }
        else if (e.KeyChar == (char)Keys.Escape)
        {
            txtEdgeWeightEditor.Visible = false;
            editingEdge = null;
            e.Handled = true;
        }
    }

    private void txtEdgeWeightEditor_LostFocus(object sender, EventArgs e)
    {
        if (!txtEdgeWeightEditor.Visible || editingEdge == null) return;

        if (int.TryParse(txtEdgeWeightEditor.Text, out int newWeight) && newWeight >= 0)
        {
            editingEdge.Weight = newWeight;
            pbxCanvas.Invalidate();
        }

        txtEdgeWeightEditor.Visible = false;
        editingEdge = null;
    }

    private void txtEdgeWeightEditor_TextChanged(object sender, EventArgs e) { }
    private void groupBox5_Enter(object sender, EventArgs e) { }
    private void label1_Click(object sender, EventArgs e) { }
    private void label2_Click(object sender, EventArgs e) { }
    private void textBox1_TextChanged(object sender, EventArgs e) { }
}