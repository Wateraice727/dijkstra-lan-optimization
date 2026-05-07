namespace dlo_winform;

public readonly struct PacketTickResult
{
    public readonly bool IsMove { get; }
    public readonly bool IsComplete { get; }
    public readonly int FromNodeId { get; }
    public readonly int ToNodeId { get; }
    public readonly long TickTravelTime { get; }
    public readonly long TotalElapsedTime { get; }

    private PacketTickResult(bool isMove, bool isComplete, int fromNodeId, int toNodeId, long tickTravelTime, long totalElapsedTime)
    {
        IsMove = isMove;
        IsComplete = isComplete;
        FromNodeId = fromNodeId;
        ToNodeId = toNodeId;
        TickTravelTime = tickTravelTime;
        TotalElapsedTime = totalElapsedTime;
    }

    public static PacketTickResult Moved(int fromNodeId, int toNodeId, long tickTravelTime, long totalElapsedTime)
    {
        return new PacketTickResult(true, false, fromNodeId, toNodeId, tickTravelTime, totalElapsedTime);
    }

    public static PacketTickResult Completed(long totalElapsedTime)
    {
        return new PacketTickResult(false, true, 0, 0, 0, totalElapsedTime);
    }
}

public sealed class PacketSimulation
{
    public readonly DijkstraRouteResult Route;
    public int CurrentEdgeIndex;
    public long ElapsedTime;

    public bool IsComplete()
    {
        return CurrentEdgeIndex >= Route.PathEdges.Count - 1;
    }
    public PacketSimulation(DijkstraRouteResult route)
    {
        Route = route;
        CurrentEdgeIndex = -1;
        ElapsedTime = 0;
    }
    public PacketTickResult Tick()
    {
        if (IsComplete()) return PacketTickResult.Completed(ElapsedTime);
        CurrentEdgeIndex++;
        if (CurrentEdgeIndex >= Route.PathEdges.Count) return PacketTickResult.Completed(ElapsedTime);

        NetworkEdge edge = Route.PathEdges[CurrentEdgeIndex];
        ElapsedTime += edge.Weight;

        return PacketTickResult.Moved(edge.StartNode.Id, edge.EndNode.Id, edge.Weight, ElapsedTime);
    }
}
