namespace Runes.Collections.Mutable
{
    public class Queue<A>
    {
        public Queue()
        {
            Head = Rear = new QueueNode(default, null);
            HeadSyncObj = new object();
            RearSyncObj = new object();
        }

        public bool IsEmpty => !NonEmpty;
        public bool NonEmpty => Peek(out _);

        public bool Dequeue(out A value)
        {
            lock (HeadSyncObj)
            {
                var firstNode = Head.Next;
                if (firstNode != null)
                {
                    Head.Next = firstNode.Next;
                    value = firstNode.Info;

                    return true;
                }
            }

            value = default;
            return false;
        }

        public void Enqueue(A info)
        {
            var newNode = new QueueNode(info, null);
            lock (RearSyncObj)
            {
                Rear.Next = newNode;
                Rear = Rear.Next;
            }
        }

        public bool Peek(out A value)
        {
            lock (HeadSyncObj)
            {
                if (Head.Next != null)
                {
                    value = Head.Next.Info;

                    return true;
                }
            }

            value = default;
            return false;
        }

        private QueueNode Head { get; set; }
        private QueueNode Rear { get; set; }

        private object HeadSyncObj { get; }
        private object RearSyncObj { get; }

        private class QueueNode
        {
            public A Info { get; }
            public QueueNode Next { get; set; }

            public QueueNode(A info, QueueNode next)
            {
                Info = info;
                Next = next;
            }
        }
    }
}
