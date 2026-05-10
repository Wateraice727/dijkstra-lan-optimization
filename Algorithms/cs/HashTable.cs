namespace FastHashTable
{
    public class MyHashTable
    {
        const int MOD = 4999999;
        const long MAXVAL = 1000000000000000000L;
        const long XOR = 727727727727727727L;
        const int MAXNODE = 2000000;
        private class Node
        {
            public long value;
            public Node next;
            public Node(long _value = 0)
            {
                value = _value;
                next = null;
            }    
        }
        private int nodeCount;
        private Node[] bucket, allNode;
        public MyHashTable() 
        {
            nodeCount = 0;
            bucket = new Node[MOD];
            for (int i = 0; i < MOD; i++) bucket[i] = null;
            allNode = new Node[MAXNODE];
        }
        private Node CreateNewNode(long val)
        {
            allNode[nodeCount] = new Node(val);
            return allNode[nodeCount++];
        }
        private int GetHashCode(long x)
        {
            x = (x + MAXVAL) ^ XOR;
            int res = (int)(x % MOD);
            return res < 0 ? res + MOD : res;
        }
        public bool Add(long x)
        {
            int h = GetHashCode(x);
            Node pointer = bucket[h];
            while (pointer != null)
            {
                if (pointer.value == x) return false;
                pointer = pointer.next;
            }
            Node newElement = CreateNewNode(x);
            newElement.next = bucket[h];
            bucket[h] = newElement;
            return true;
        }
    }
    public class MyDictionary
    {
        const int MOD = 4999999;
        const long MAXVAL = 1000000000000000000L;
        const long XOR = 727727727727727727L;
        const int MAXNODE = 2000000;

        private class Node
        {
            public long key;
            public dlo_winform.NetworkEdge value;
            public Node next;

            public Node(long _key, dlo_winform.NetworkEdge _value)
            {
                key = _key;
                value = _value;
                next = null;
            }
        }
        private int nodeCount;
        private Node[] bucket;
        private Node[] allNode;
        public MyDictionary()
        {
            nodeCount = 0;
            bucket = new Node[MOD];
            for (int i = 0; i < MOD; i++) bucket[i] = null;
            allNode = new Node[MAXNODE];
        }
        private Node CreateNewNode(long k, dlo_winform.NetworkEdge v)
        {
            allNode[nodeCount] = new Node(k, v);
            return allNode[nodeCount++];
        }
        private int GetHashCode(long x)
        {
            x = (x + MAXVAL) ^ XOR;
            int res = (int)(x % MOD);
            return res < 0 ? res + MOD : res;
        }
        public bool Add(long k, ref dlo_winform.NetworkEdge v)
        {
            int h = GetHashCode(k);
            Node pointer = bucket[h];
            while (pointer != null)
            {
                if (pointer.key == k)
                {
                    v = pointer.value;
                    return false;
                }
                pointer = pointer.next;
            }
            Node newElement = CreateNewNode(k, v);
            newElement.next = bucket[h];
            bucket[h] = newElement;
            return true;
        }
    }
}