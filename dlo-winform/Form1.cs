using System.ComponentModel;
using System.Diagnostics;

namespace dlo_winform;

public partial class Form1 : Form
{
    private const int AnimationTickMilliseconds = 500;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GraphData GD = null!;
    private System.Windows.Forms.Timer animationTimer = null!;
    private DijkstraRouteResult? currentRoute;
    private PacketSimulation? currentSimulation;
    private bool isPaused = false;
    private bool isAddNodeMode = false;
    private bool isRemoveNodeMode = false;
    private NetworkEdge? editingEdge = null;
    private double lastCalcMs;
    private bool loadingSampleList = true;

    public Form1()
    {
        InitializeComponent();
        pbxCanvas.Paint += pbxCanvas_Paint;
        pbxCanvas.MouseDown += pbxCanvas_MouseDown;
        pbxCanvas.MouseMove += pbxCanvas_MouseMove;
        pbxCanvas.MouseUp += pbxCanvas_MouseUp;
        pbxCanvas.MouseDoubleClick += pbxCanvas_MouseDoubleClick;

        if (!DesignMode)
        {
            this.DoubleBuffered = true;
            GD = SampleGraphs.CreateAt(0);
            cmbSampleGraphs.Items.AddRange(SampleGraphs.Names.ToArray());
            cmbSampleGraphs.SelectedIndex = 0;
            loadingSampleList = false;
            cmbSampleGraphs.SelectedIndexChanged += cmbSampleGraphs_SelectedIndexChanged;
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = AnimationTickMilliseconds;
            animationTimer.Tick += AnimationTimer_Tick;
        }
    }

    private void cmbSampleGraphs_SelectedIndexChanged(object? sender, EventArgs e)
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

    private void AnimationTimer_Tick(object? sender, EventArgs e)
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

    private readonly Font defaultFont = SystemFonts.DefaultFont;
    private readonly Font boldFont = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size, FontStyle.Bold);
    private readonly Pen edgePen = new Pen(Color.Black, 2), pathPen = new Pen(Color.DodgerBlue, 3);
    private void pbxCanvas_Paint(object sender, PaintEventArgs e)
    {
        if (GD == null) return;
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        foreach (NetworkEdge edge in GD.edgeList)
        {
            bool isInPath = currentRoute != null && currentRoute.PathEdges.Contains(edge);
            Pen pen = isInPath ? pathPen : edgePen;
            g.DrawLine(pen, edge.StartNode.Position.X, edge.StartNode.Position.Y, edge.EndNode.Position.X, edge.EndNode.Position.Y);

            PointF midPoint = new PointF(
                (edge.StartNode.Position.X + edge.EndNode.Position.X) / 2,
                (edge.StartNode.Position.Y + edge.EndNode.Position.Y) / 2);
            string speedText = GraphEditor.SpeedToMbpsString(edge.TransferSpeedBytesPerSecond);
            g.DrawString(speedText, defaultFont, Brushes.Black, midPoint);
        }

        if (currentSimulation != null && currentRoute != null && currentSimulation.CurrentEdgeIndex >= 0 && currentSimulation.CurrentEdgeIndex < currentRoute.PathEdges.Count)
        {
            NetworkEdge edge = currentRoute.PathEdges[currentSimulation.CurrentEdgeIndex];
            PointF start = edge.StartNode.Position;
            PointF end = edge.EndNode.Position;
            float packetX = (start.X + end.X) / 2;
            float packetY = (start.Y + end.Y) / 2;
            g.FillEllipse(Brushes.Red, packetX - 5, packetY - 5, 10, 10);
        }

        foreach (NetworkNode node in GD.nodeList)
        {
            float x = node.Position.X - node.Radius;
            float y = node.Position.Y - node.Radius;
            float diameter = node.Radius * 2;
            Brush nodeBrush = Brushes.LightBlue;

            if (currentRoute != null && node.Id == currentRoute.StartNodeId)
                nodeBrush = Brushes.Green;
            else if (currentRoute != null && node.Id == currentRoute.DestinationNodeId)
                nodeBrush = Brushes.Orange;

            g.FillEllipse(nodeBrush, x, y, diameter, diameter);
            g.DrawEllipse(Pens.DarkBlue, x, y, diameter, diameter);

            string idText = node.Id.ToString();
            SizeF textSize = g.MeasureString(idText, boldFont);
            float textX = node.Position.X - textSize.Width / 2;
            float textY = node.Position.Y - textSize.Height / 2;
            g.DrawString(idText, boldFont, Brushes.Black, textX, textY);
        }
    }

    private void pbxCanvas_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        if (isAddNodeMode)
        {
            NetworkNode node = GraphEditor.AddNode(GD, e.Location);
            pbxCanvas.Invalidate();
            Log("Added node " + node.Id + " at (" + e.Location.X + ", " + e.Location.Y + ")");
            isAddNodeMode = false;
            return;
        }

        if (isRemoveNodeMode)
        {
            (bool nodeRemoved, int removedCount, List<NetworkEdge> removedEdges) = GraphEditor.RemoveNode(GD, e.Location);
            if (nodeRemoved)
            {
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
            isRemoveNodeMode = false;
            return;
        }

        if (checkBoxEditWeight.Checked)
        {
            NetworkEdge edge = GraphEditor.TryGetEdgeNearPoint(GD, e.Location, 6f);
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
            NetworkNode node = GD.GetNode(e.Location);
            if (node != null)
                pbxCanvas.Tag = node;
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
    }

    private void pbxCanvas_MouseMove(object? sender, MouseEventArgs e)
    {
    }

    private void pbxCanvas_MouseUp(object? sender, MouseEventArgs e)
    {
        if (checkBox1.Checked && pbxCanvas.Tag is NetworkNode startNode && e.Button == MouseButtons.Left)
        {
            var endNode = GD.GetNode(e.Location);
            if (endNode != null && startNode != endNode)
            {
                ToggleEdgeOutcome outcome = GraphEditor.ToggleEdge(GD, startNode, endNode);
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

    private void checkBoxEditWeight_CheckedChanged(object? sender, EventArgs e)
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
        GD = SampleGraphs.CreateAt(index);
        foreach (NetworkEdge edge in GD.edgeList)
        {
            if (edge.TransferSpeedBytesPerSecond == 0)
                edge.TransferSpeedBytesPerSecond = GraphEditor.GenerateTransferSpeed();
        }
        currentRoute = null;
        currentSimulation = null;
        animationTimer.Stop();
        txtLog.Clear();
        Log("Loaded sample graph: " + cmbSampleGraphs.Text);
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

    private void pbxCanvas_MouseDoubleClick(object? sender, MouseEventArgs e)
    {
    }

    private void txtEdgeWeightEditor_KeyPress(object? sender, KeyPressEventArgs e)
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

    private void txtEdgeWeightEditor_LostFocus(object? sender, EventArgs e)
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
}
