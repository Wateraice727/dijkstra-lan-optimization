using Tree;
using DisjointSetUnion;
using FastHashTable;

namespace Graph
{
    public struct EdgeGraph
    {
        public struct Edge
        {
            public int to;
            public long weight;
        }
        const long INF = 1000000000000000000L;
        private int vertex;
        public List<Edge>[] adjList;
        private int[] trace;
        private MinHeap myHeap;
        public EdgeGraph(int _vertex = 0)
        {
            vertex = _vertex;
            myHeap = new MinHeap(1000000);
            adjList = new List<Edge>[vertex + 1];
            trace = new int[vertex + 1];
            for (int i = 0; i <= vertex; i++) adjList[i] = new List<Edge>();
            for (int i = 0; i < trace.Length; i++) trace[i] = -1;
        }
        public long Infinity
        {
            get
            {
                return INF;
            }
        }
        public void AddEdge(int _from, int _to, long _weight, bool isDirected = false)
        {
            Edge e = new Edge()
            {
                to = _to,
                weight = _weight
            };
            adjList[_from].Add(e);
            if (!isDirected)
            {
                e.to = _from;
                adjList[_to].Add(e);
            }
        }
        public long[] DenseDijkstra(int start)
        {
            long[] distance = new long[vertex + 1];
            for (int i = 1; i < distance.Length; i++) distance[i] = INF;
            distance[start] = 0;
            bool[] visited = new bool[vertex + 1];
            for (int i = 1; i <= vertex; i++)
            {
                int from = -1;
                for (int j = 0; j <= vertex; j++) if (!visited[j] && (from == -1 || distance[j] < distance[from])) from = j;
                if (distance[from] == INF) break;
                visited[from] = true;
                foreach (Edge edge in adjList[from])
                {
                    int to = edge.to;
                    long length = edge.weight;
                    if (distance[from] + length < distance[to])
                    {
                        distance[to] = distance[from] + length;
                        trace[to] = from;
                    }
                }
            }
            return distance;
        }
        public long[] SparseDijkstra(int start)
        {
            long[] distance = new long[vertex + 1];
            for (int i = 0; i < distance.Length; i++) distance[i] = INF;
            distance[start] = 0;
            myHeap.Enqueue(start, distance[start]);
            while (!myHeap.IsEmpty())
            {
                int from = myHeap.PeekVertex();
                long distance_from = myHeap.PeekWeight();
                myHeap.Dequeue();
                if (distance_from != distance[from]) continue;
                foreach (Edge edge in adjList[from])
                {
                    int to = edge.to;
                    long length = edge.weight;
                    if (distance[from] + length < distance[to])
                    {
                        distance[to] = distance[from] + length;
                        trace[to] = from;
                        myHeap.Enqueue(to, distance[to]);
                    }
                }
            }
            return distance;
        }
        public List<int> TracePath(int start, int end)
        {
            if (end != start && trace[end] == -1) return new List<int>();
            List<int> path = new List<int>();
            while (end != -1)
            {
                path.Add(end);
                end = trace[end];
            }
            path.Reverse();
            return path;
        }
        private struct FullEdge
        {
            public int from, to;
            public long weight;
        }
        public void RandomGenerate(int edge, bool isDirected = false)
        {
            List<FullEdge> edgeList = new List<FullEdge>();
            DSU dsu = new DSU(vertex);
            MyHashTable ht = new MyHashTable();
            int count = 0;
            Random rand = new Random();
            if (edge >= vertex - 1) 
            {   while (count < vertex - 1)
                {
                    int u = rand.Next(1, vertex + 1), v = rand.Next(1, vertex + 1);
                    if (u != v && dsu.UnionSet(u, v))
                    {
                        long w = rand.Next(1, 1025);
                        FullEdge e = new FullEdge()
                        {
                            from = u,
                            to = v,
                            weight = w
                        };
                        edgeList.Add(e);
                        count++;
                    }
                }
                foreach (FullEdge e in edgeList)
                {
                    long key = 1L * Math.Min(e.from, e.to) * (vertex + 1) + Math.Max(e.from, e.to);
                    ht.Add(key);
                }
            }
            long maxEdge = 1L * vertex * (vertex - 1) / 2;
            if (edge > maxEdge) edge = (int)maxEdge;
            while (count < edge)
            {
                int u = rand.Next(1, vertex + 1), v = rand.Next(1, vertex + 1);
                if (u == v) continue;
                long key = 1L * Math.Min(u, v) * (vertex + 1) + Math.Max(u, v);
                if (ht.Add(key))
                {
                    long w = rand.Next(1, 1024);
                    FullEdge e = new FullEdge()
                    {
                        from = u,
                        to = v,
                        weight = w
                    };
                    edgeList.Add(e);
                    count++;
                }
            }
            //foreach (FullEdge e in edgeList)
            //{
            //    Console.Write(e.from);
            //    Console.Write(" ");
            //    Console.Write(e.to);
            //    Console.Write(" ");
            //    Console.Write(e.weight);
            //    Console.Write("\n");
            //}
            foreach (FullEdge e in edgeList) AddEdge(e.from, e.to, e.weight, isDirected);
        }
    }
}
