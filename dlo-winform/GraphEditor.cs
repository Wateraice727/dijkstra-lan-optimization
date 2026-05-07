using FastHashTable;

namespace dlo_winform;

public static class GraphEditor
{
    private static readonly Random _rng = new Random();

    public static void AddEdgeIfMissing(GraphData data, NetworkNode startNode, NetworkNode endNode, int weight)
    {
        if (startNode == null || endNode == null || startNode == endNode) return;

        bool exists = false;

        foreach (NetworkEdge edge in data.edgeList) if ((edge.StartNode == startNode && edge.EndNode == endNode) || (edge.StartNode == endNode && edge.EndNode == startNode))
        {
            exists = true;
            break;
        }

        if (!exists) data.edgeList.Add(new NetworkEdge
        {
            StartNode = startNode,
            EndNode = endNode,
            Weight = weight,
            TransferSpeedBytesPerSecond = GenerateTransferSpeed()
        });
    }

    public static ToggleEdgeOutcome ToggleEdge(GraphData data, NetworkNode nodeA, NetworkNode nodeB)
    {
        return ToggleEdge(data, nodeA, nodeB, GenerateTransferSpeed());
    }

    public static ToggleEdgeOutcome ToggleEdge(GraphData data, NetworkNode nodeA, NetworkNode nodeB, long speed)
    {
        if (nodeA == null || nodeB == null || nodeA == nodeB) return ToggleEdgeOutcome.NoAction;

        NetworkEdge edge = null;
        foreach (NetworkEdge e in data.edgeList) if ((e.StartNode == nodeA && e.EndNode == nodeB) || (e.StartNode == nodeB && e.EndNode == nodeA))
        {
            edge = e;
            break;
        }

        if (edge != null)
        {
            data.edgeList.Remove(edge);
            return ToggleEdgeOutcome.Removed;
        }

        data.edgeList.Add(new NetworkEdge { StartNode = nodeA, EndNode = nodeB, Weight = 1, TransferSpeedBytesPerSecond = speed });
        return ToggleEdgeOutcome.Created;
    }

    public static NetworkEdge TryGetEdgeNearPoint(GraphData data, PointF point, float tolerance)
    {
        foreach (NetworkEdge edge in data.edgeList)
        {
            float dist = GeometryHelpers.DistanceToSegment(point, edge.StartNode.Position, edge.EndNode.Position);
            if (dist < tolerance)
                return edge;
        }
        return null;
    }

    public static bool CanPlaceNode(GraphData data, PointF position, int radius)
    {
        foreach (NetworkNode node in data.nodeList)
        {
            if (GeometryHelpers.CirclesOverlap(position, radius, node.Position, node.Radius))
                return false;
        }
        return true;
    }

    public static NetworkNode AddNode(GraphData data, PointF position)
    {
        MyHashTable usedIds = new MyHashTable();
        foreach (NetworkNode n in data.nodeList) usedIds.Add(n.Id);
        int id = 1;
        while (!usedIds.Add(id)) id++;
        NetworkNode node = new NetworkNode { Id = id, Position = position };
        data.nodeList.Add(node);
        return node;
    }

    public static (bool nodeRemoved, int removedEdgeCount, List<NetworkEdge> removedEdges) RemoveNode(GraphData data, PointF position)
    {
        NetworkNode node = data.GetNode(Point.Round(position));
        if (node == null) return (false, 0, new List<NetworkEdge>());

        List<NetworkEdge> removedEdges = new List<NetworkEdge>();
        for (int i = data.edgeList.Count - 1; i >= 0; i--)
        {
            NetworkEdge edge = data.edgeList[i];
            if (edge.StartNode == node || edge.EndNode == node)
            {
                removedEdges.Add(edge);
                data.edgeList.RemoveAt(i);
            }
        }

        data.nodeList.Remove(node);
        return (true, removedEdges.Count, removedEdges);
    }

    public static long GenerateTransferSpeed()
    {
        long[] bases = { 12_500_000L, 37_500_000L, 125_000_000L, 1_250_000_000L };
        long baseSpeed = bases[_rng.Next(bases.Length)];
        double factor = 0.5 + _rng.NextDouble();
        long speed = (long)(baseSpeed * factor);
        return speed > 1_000_000L ? speed : 1_000_000L;
    }

    public static long PacketSizeBytes(int value, PacketUnit unit)
    {
        if (value <= 0) return 0;
        switch (unit)
        {
            case PacketUnit.Bytes: return value;
            case PacketUnit.KB: return value * 1024L;
            case PacketUnit.MB: return value * 1024L * 1024L;
            case PacketUnit.GB: return value * 1024L * 1024L * 1024L;
            default: return value;
        }
    }

    public static int ComputeEdgeWeightMs(long packetBytes, long speedBytesPerSec)
    {
        if (speedBytesPerSec <= 0) return int.MaxValue;
        long ms = packetBytes * 1000L / speedBytesPerSec;
        return ms > 0 ? (int)ms : 1;
    }

    public static void RecalculateEdgeWeights(GraphData data, long packetBytes)
    {
        foreach (NetworkEdge edge in data.edgeList)
        {
            edge.Weight = ComputeEdgeWeightMs(packetBytes, edge.TransferSpeedBytesPerSecond);
        }
    }

    public static string SpeedToMbpsString(long speedBytesPerSec)
    {
        double mbps = speedBytesPerSec / 1_000_000.0;
        return mbps.ToString("F1");
    }
}
