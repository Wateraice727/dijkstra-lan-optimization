using FastHashTable;
using Graph;

namespace dlo_winform;

public static class SampleGraphs
{
    public static IReadOnlyList<string> Names { get; } = new[]
    {
        "Empty Graph",
        "Linear 5",
        "Star Hub",
        "Ring 6",
        "Mesh 7",
        "Two Clusters",
        "Weighted Detour",
        "Disconnected Demo",
        "Grid 4x4",
        "Diamond 54",
        "Cluster 64",
        "MeshWeb 76",
        "Lattice 88"
    };
    public static void NormalizeCoordinates(GraphData data, int canvasWidth, int canvasHeight)
    {
        if (data.nodeList.Count == 0) return;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (NetworkNode node in data.nodeList)
        {
            if (node.Position.X < minX) minX = node.Position.X;
            if (node.Position.X > maxX) maxX = node.Position.X;
            if (node.Position.Y < minY) minY = node.Position.Y;
            if (node.Position.Y > maxY) maxY = node.Position.Y;
        }
        float w = maxX - minX;
        float h = maxY - minY;
        if (w == 0f) w = 1f;
        if (h == 0f) h = 1f;
        float targetW = canvasWidth * 0.9f;
        float targetH = canvasHeight * 0.9f;
        float scale = targetW / w < targetH / h ? targetW / w : targetH / h;
        if (scale > 1.2f) scale = 1.2f;

        float cx = (minX + maxX) / 2f;
        float cy = (minY + maxY) / 2f;
        float targetCx = canvasWidth / 2f;
        float targetCy = canvasHeight / 2f;

        foreach (NetworkNode node in data.nodeList)
        {
            float nx = (node.Position.X - cx) * scale + targetCx;
            float ny = (node.Position.Y - cy) * scale + targetCy;
            node.Position = new PointF(nx, ny);
        }
    }
    public static GraphData CreateAt(int index, int canvasWidth, int canvasHeight)
    {
        GraphData data = index switch
        {
            0 => EmptyGraph(),
            1 => Linear5(),
            2 => StarHub(),
            3 => Ring6(),
            4 => Mesh7(),
            5 => TwoClusters(),
            6 => WeightedDetour(),
            7 => DisconnectedDemo(),
            8 => Grid16(),
            9 => Diamond54(),
            10 => Cluster64(),
            11 => MeshWeb76(),
            12 => Lattice88(),
            _ => EmptyGraph()
        };
        NormalizeCoordinates(data, canvasWidth, canvasHeight);
        return data;
    }

    public static int NextIndex(int current) => (current + 1) % Names.Count;
    public static int PreviousIndex(int current) => (current + Names.Count - 1) % Names.Count;

    private static void AddEdge(GraphData data, int a, int b)
    {
        if (a == b) return;
        if (a > b) { int t = a; a = b; b = t; }
        data.edgeList.Add(new NetworkEdge
        {
            StartNode = data.nodeList[a],
            EndNode = data.nodeList[b],
            Weight = 1 + (a * 7 + b * 13) % 20
        });
    }

    private static GraphData EmptyGraph()
    {
        return new GraphData();
    }

    private static GraphData Build(PointF[] positions, (int from, int to, int weight)[] edges)
    {
        GraphData data = new GraphData();
        for (int i = 0; i < positions.Length; i++)
            data.nodeList.Add(new NetworkNode { Id = i + 1, Position = positions[i], Radius = 10 });
        foreach ((int from, int to, int weight) e in edges)
            data.edgeList.Add(new NetworkEdge
            {
                StartNode = data.nodeList[e.from],
                EndNode = data.nodeList[e.to],
                Weight = e.weight
            });
        return data;
    }


    private static GraphData Linear5()
    {
        return Build(
            new[] { new PointF(90, 200), new PointF(200, 200), new PointF(310, 200), new PointF(420, 200), new PointF(530, 200) },
            new[] { (0, 1, 5), (1, 2, 3), (2, 3, 7), (3, 4, 2) }
        );
    }

    private static GraphData StarHub()
    {
        return Build(
            new[] { new PointF(320, 200), new PointF(80, 200), new PointF(160, 60), new PointF(320, 30), new PointF(480, 60), new PointF(560, 200), new PointF(320, 370) },
            new[] { (0, 1, 1), (0, 2, 2), (0, 3, 3), (0, 4, 2), (0, 5, 1), (0, 6, 4) }
        );
    }

    private static GraphData Ring6()
    {
        return Build(
            new[] { new PointF(320, 30), new PointF(520, 130), new PointF(520, 310), new PointF(320, 390), new PointF(120, 310), new PointF(120, 130) },
            new[] { (0, 1, 3), (1, 2, 4), (2, 3, 3), (3, 4, 5), (4, 5, 4), (5, 0, 6) }
        );
    }

    private static GraphData Mesh7()
    {
        return Build(
            new[] { new PointF(160, 100), new PointF(320, 80), new PointF(480, 100), new PointF(120, 270), new PointF(320, 280), new PointF(520, 270), new PointF(320, 380) },
            new[] { (0, 1, 2), (1, 2, 3), (0, 3, 4), (1, 4, 1), (2, 5, 3), (3, 4, 2), (4, 5, 2), (4, 6, 5), (3, 6, 3), (1, 3, 4) }
        );
    }

    private static GraphData TwoClusters()
    {
        return Build(
            new[] { new PointF(110, 140), new PointF(80, 280), new PointF(200, 210), new PointF(440, 140), new PointF(420, 280), new PointF(540, 210) },
            new[] { (0, 1, 3), (0, 2, 2), (1, 2, 1), (3, 4, 3), (3, 5, 2), (4, 5, 1), (2, 4, 10) }
        );
    }

    private static GraphData WeightedDetour()
    {
        return Build(
            new[] { new PointF(80, 200), new PointF(200, 200), new PointF(320, 200), new PointF(440, 130), new PointF(560, 200) },
            new[] { (0, 1, 5), (1, 2, 20), (2, 3, 5), (3, 4, 5), (1, 3, 10) }
        );
    }

    private static GraphData DisconnectedDemo()
    {
        return Build(
            new[] { new PointF(120, 200), new PointF(200, 140), new PointF(200, 260), new PointF(440, 200), new PointF(520, 200) },
            new[] { (0, 1, 2), (0, 2, 3), (1, 2, 1), (3, 4, 5) }
        );
    }

    private static GraphData Grid16()
    {
        var data = new GraphData();
        int cols = 4, rows = 4, margin = 30;
        float sx = (640f - 2 * margin) / (cols - 1);
        float sy = (420f - 2 * margin) / (rows - 1);
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                data.nodeList.Add(new NetworkNode { Id = data.nodeList.Count + 1, Position = new PointF(margin + c * sx, margin + r * sy), Radius = 10 });
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                int i = r * cols + c;
                if (c + 1 < cols) AddEdge(data, i, i + 1);
                if (r + 1 < rows) AddEdge(data, i, i + cols);
                if (c + 1 < cols && r + 1 < rows) AddEdge(data, i, i + cols + 1);
            }
        return data;
    }

    private static GraphData Diamond54()
    {
        var data = new GraphData();
        int[] lw = { 3, 5, 7, 9, 11, 9, 7, 3 };
        int margin = 30;
        float sy = (420f - 2 * margin) / (lw.Length - 1);
        for (int row = 0; row < lw.Length; row++)
        {
            int w = lw[row];
            float sx = (640f - 2 * margin) / (w + 1);
            float y = margin + row * sy;
            for (int c = 0; c < w; c++)
                data.nodeList.Add(new NetworkNode { Id = data.nodeList.Count + 1, Position = new PointF(margin + (c + 1) * sx, y), Radius = 10 });
        }
        int offset = 0;
        for (int row = 0; row < lw.Length; row++)
        {
            int w = lw[row];
            for (int c = 0; c < w; c++)
            {
                int i = offset + c;
                if (c > 0) AddEdge(data, i, i - 1);
                if (row + 1 < lw.Length)
                {
                    int nextOff = offset + w;
                    int nw = lw[row + 1];
                    float myX = data.nodeList[i].Position.X;
                    float nStep = (640f - 2 * margin) / (nw + 1);
                    for (int nc = 0; nc < nw; nc++)
                    {
                        int j = nextOff + nc;
                        if (Math.Abs(data.nodeList[j].Position.X - myX) < nStep * 1.3f)
                            AddEdge(data, i, j);
                    }
                }
            }
            offset += w;
        }
        return data;
    }

    private static GraphData Cluster64()
    {
        var data = new GraphData();
        int cw = 4, ch = 4, gw = 150, gh = 170;
        int[][] centers = { new[] { 40, 40 }, new[] { 450, 40 }, new[] { 40, 230 }, new[] { 450, 230 } };
        foreach (var cent in centers)
        {
            float sx = (float)gw / (cw - 1);
            float sy = (float)gh / (ch - 1);
            int baseIdx = data.nodeList.Count;
            for (int r = 0; r < ch; r++)
                for (int c = 0; c < cw; c++)
                    data.nodeList.Add(new NetworkNode { Id = data.nodeList.Count + 1, Position = new PointF(cent[0] + c * sx, cent[1] + r * sy), Radius = 10 });
            for (int r = 0; r < ch; r++)
                for (int c = 0; c < cw; c++)
                {
                    int i = baseIdx + r * cw + c;
                    if (c > 0) AddEdge(data, i, i - 1);
                    if (r > 0) AddEdge(data, i, i - cw);
                    if (c > 0 && r > 0) AddEdge(data, i, i - cw - 1);
                }
        }
        AddEdge(data, 3, 19); AddEdge(data, 7, 23);
        AddEdge(data, 23, 35); AddEdge(data, 39, 55);
        AddEdge(data, 40, 45); AddEdge(data, 15, 50);
        AddEdge(data, 30, 60); AddEdge(data, 0, 63);
        return data;
    }

    private static GraphData MeshWeb76()
    {
        var data = new GraphData();
        int[] colSizes = { 12, 10, 8, 6, 8, 10, 12, 10 };
        int margin = 20;
        float sx = (640f - 2 * margin) / (colSizes.Length - 1);
        for (int ci = 0; ci < colSizes.Length; ci++)
        {
            int cnt = colSizes[ci];
            float sy = (420f - 2 * margin) / (cnt - 1);
            for (int ri = 0; ri < cnt; ri++)
                data.nodeList.Add(new NetworkNode { Id = data.nodeList.Count + 1, Position = new PointF(margin + ci * sx, margin + ri * sy), Radius = 10 });
        }
        Random rng = new Random(76);
        int offset = 0;
        for (int ci = 0; ci < colSizes.Length; ci++)
        {
            int cnt = colSizes[ci];
            for (int ri = 0; ri < cnt; ri++)
            {
                int i = offset + ri;
                if (ri > 0) AddWeightedEdge(data, i, i - 1, rng);
                if (ci > 0)
                {
                    int prevCnt = colSizes[ci - 1];
                    int prevOff = offset - prevCnt;
                    float myY = data.nodeList[i].Position.Y;
                    float prevSy = (420f - 2 * margin) / Math.Max(prevCnt - 1, 1);
                    float bestD = float.MaxValue;
                    int best = -1;
                    for (int pr = 0; pr < prevCnt; pr++)
                    {
                        int pj = prevOff + pr;
                        float d = Math.Abs(data.nodeList[pj].Position.Y - myY);
                        if (d < bestD) { bestD = d; best = pj; }
                    }
                    if (best >= 0) AddWeightedEdge(data, best, i, rng);
                    if (best + 1 < offset && Math.Abs(data.nodeList[best + 1].Position.Y - myY) < prevSy * 0.6f)
                        AddWeightedEdge(data, best + 1, i, rng);
                }
            }
            offset += cnt;
        }
        return data;
    }

    private static void AddWeightedEdge(GraphData data, int a, int b, Random rng)
    {
        if (a == b) return;
        if (a > b) { int t = a; a = b; b = t; }
        data.edgeList.Add(new NetworkEdge
        {
            StartNode = data.nodeList[a],
            EndNode = data.nodeList[b],
            Weight = rng.Next(20, 101)
        });
    }

    private static GraphData Lattice88()
    {
        var data = new GraphData();
        int cols = 8, rows = 11, margin = 20;
        float sx = (640f - 2 * margin) / (cols - 1);
        float sy = (420f - 2 * margin) / (rows - 1);
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                data.nodeList.Add(new NetworkNode { Id = data.nodeList.Count + 1, Position = new PointF(margin + c * sx, margin + r * sy), Radius = 10 });
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                int i = r * cols + c;
                if (c > 0) AddEdge(data, i, i - 1);
                if (r > 0) AddEdge(data, i, i - cols);
                if (c > 0 && r > 0) AddEdge(data, i, i - cols - 1);
                if (c > 0 && r > 0 && (r + c) % 3 == 0) AddEdge(data, i, i - cols - 2);
                if (c + 1 < cols && r > 0 && (r + c) % 2 == 0) AddEdge(data, i, i - cols + 1);
            }
        return data;
    }
    public static GraphData GenerateMassiveGraph(int nodeCount, int edgeCount, int canvasWidth, int canvasHeight)
    {
        GraphData data = new GraphData();
        Random rng = new Random();

        EdgeGraph logicGraph = new EdgeGraph(nodeCount);
        logicGraph.RandomGenerate(edgeCount, false);

        float cx = canvasWidth / 2f;
        float cy = canvasHeight / 2f;
        float radiusX = canvasWidth * 0.45f;
        float radiusY = canvasHeight * 0.45f;

        for (int i = 1; i <= nodeCount; i++)
        {
            float x = rng.Next((int)(cx - radiusX), (int)(cx + radiusX));
            float y = rng.Next((int)(cy - radiusY), (int)(cy + radiusY));
            data.nodeList.Add(new NetworkNode
            {
                Id = i,
                Position = new PointF(x, y),
                Radius = nodeCount > 1000 ? 1 : 10
            });
        }
        for (int u = 1; u <= nodeCount; u++)
            foreach (var edge in logicGraph.adjList[u]) if (u < edge.to) data.edgeList.Add(new NetworkEdge
            {
                StartNode = data.nodeList[u - 1],
                EndNode = data.nodeList[edge.to - 1],
                Weight = (int)edge.weight
            });

        return data;
    }
}
