namespace dlo_winform;

public class NetworkNode 
{
    public int Id { get; set; } 
    public PointF Position { get; set; } 
    public int Radius { get; set; } = 10; 
}

public class NetworkEdge 
{
    public NetworkNode StartNode { get; set; } = null!;
    public NetworkNode EndNode { get; set; } = null!;
    public int Weight { get; set; }
    public long TransferSpeedBytesPerSecond { get; set; }
}

public class GraphData
{
    public List<NetworkNode> nodeList { get; } = new List<NetworkNode>();
    public List<NetworkEdge> edgeList { get; } = new List<NetworkEdge>();

    public NetworkNode GetNode(Point mousePos)
    {
        foreach (NetworkNode node in nodeList)
        {
            float dx = mousePos.X - node.Position.X;
            float dy = mousePos.Y - node.Position.Y;
            float distSq = dx * dx + dy * dy;
            if (distSq <= node.Radius * node.Radius) return node;
        }
        return null;
    }

    public NetworkNode GetNodeById(int id)
    {
        foreach (NetworkNode node in nodeList) if (node.Id == id) return node;
        return null;
    }
}

public enum ToggleEdgeOutcome
{
    Created,
    Removed,
    NoAction
}

public enum PacketUnit
{
    Bytes,
    KB,
    MB,
    GB
}
