using FastHashTable;
using Graph;

namespace dlo_winform;

public sealed class DijkstraRouteResult
{
    public int StartNodeId { get; init; }
    public int DestinationNodeId { get; init; }
    public bool Reachable { get; init; }
    public long TotalTime { get; init; }
    public IReadOnlyList<int> PathNodeIds { get; init; } = new List<int>();
    public IReadOnlyList<NetworkEdge> PathEdges { get; init; } = new List<NetworkEdge>();
}

public static class DijkstraGraphService
{
    public static DijkstraRouteResult FindRoute(GraphData data, int startNodeId, int endNodeId)
    {
        if (data.nodeList.Count == 0 || data.edgeList.Count == 0) return new DijkstraRouteResult
        {
            StartNodeId = startNodeId,
            DestinationNodeId = endNodeId
        };
        int maxId = 0;
        NetworkNode startNode = data.GetNodeById(startNodeId);
        NetworkNode endNode = data.GetNodeById(endNodeId);
        if (startNode == null || endNode == null) return new DijkstraRouteResult
        {
            StartNodeId = startNodeId,
            DestinationNodeId = endNodeId
        };
        foreach (NetworkNode node in data.nodeList) if (node.Id > maxId) maxId = node.Id;

        EdgeGraph graph = new EdgeGraph(maxId);
        MyDictionary map = new MyDictionary();

        foreach (NetworkEdge edge in data.edgeList)
        {
            if (edge.Weight < 0) continue;
            int u = edge.StartNode.Id, v = edge.EndNode.Id;
            graph.AddEdge(u, v, edge.Weight);
            long key = 1L * (u < v ? u : v) * (maxId + 1) + (u > v ? u : v);
            NetworkEdge refEdge = edge;
            map.Add(key, ref refEdge);
        }

        long[] distances = graph.SparseDijkstra(startNodeId);

        if (distances[endNodeId] >= graph.Infinity) return new DijkstraRouteResult
        {
            StartNodeId = startNodeId,
            DestinationNodeId = endNodeId,
            Reachable = false
        };

        List<int> pathIds = graph.TracePath(startNodeId, endNodeId);
        int capacity = pathIds.Count > 0 ? pathIds.Count - 1 : 0;
        List<NetworkEdge> pathEdges = new List<NetworkEdge>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            int u = pathIds[i], v = pathIds[i + 1];
            long key = 1L * (u < v ? u : v) * (maxId + 1) + (u > v ? u : v);
            NetworkEdge edge = null;
            if (!map.Add(key, ref edge)) pathEdges.Add(edge);
        }

        return new DijkstraRouteResult
        {
            StartNodeId = startNodeId,
            DestinationNodeId = endNodeId,
            Reachable = true,
            TotalTime = distances[endNodeId],
            PathNodeIds = pathIds,
            PathEdges = pathEdges
        };
    }
}
