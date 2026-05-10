namespace Tree
{ 
    public struct MinHeap
    {
        private struct Node
        {
            public int vertex;
            public long weight;
        }
        private Node[] heap;
        private int size;
        public MinHeap(int capacity = 1000000)
        {
            heap = new Node[capacity];
            size = 0;
        }
        public bool IsEmpty()
        {
            return size == 0;
        }
        public void Enqueue(int _vertex, long _weight)
        {
            Node newNode = new Node()
            {
                vertex = _vertex,
                weight = _weight
            };
            heap[size++] = newNode;
            int index = size - 1;
            while (index > 0 && heap[(index - 1) >> 1].weight > heap[index].weight)
            {
                Node temp = heap[index];
                heap[index] = heap[(index - 1) >> 1];
                heap[(index - 1) >> 1] = temp;
                index = (index - 1) >> 1;
            } 
        }
        public void Dequeue()
        {
            if (IsEmpty()) throw new InvalidOperationException("Empty Queue.");
            int index = 0;
            heap[index] = heap[--size];
            while(true)
            {
                int left = (index << 1) + 1, right = (index << 1) + 2, min = index;
                if (left < size && heap[left].weight < heap[min].weight) min = left;
                if (right < size && heap[right].weight < heap[min].weight) min = right;
                if (min != index)
                {
                    Node temp = heap[index];
                    heap[index] = heap[min];
                    heap[min] = temp;
                    index = min;
                }
                else break;
            }
        }
        public int PeekVertex()
        {
            if (IsEmpty()) throw new InvalidOperationException("Empty Queue.");
            return heap[0].vertex;
        }
        public long PeekWeight()
        {
            if (IsEmpty()) throw new InvalidOperationException("Empty Queue.");
            return heap[0].weight;
        }
    }
}
