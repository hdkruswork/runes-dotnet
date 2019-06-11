using static Runes.Predef;

namespace Runes.Collections.Mutable
{
    public class Queue<A>
    {
        public Queue()
        {
            Head = Rear = new QueueNode(default, null);
        }

        public bool IsEmpty => Head.Next == null;
        public bool NonEmpty => !IsEmpty;

        public void Enqueue(A info)
        {
            lock (Rear)
            {
                Rear.Next = new QueueNode(info, null);
                Rear = Rear.Next;
            }
        }

        public Option<A> Peek()
        {
            lock (Head)
            {
                return InnerPeek();
            }
        }
        public Option<A> Dequeue()
        {
            lock (Head)
            {
                var res = InnerPeek();
                if (Head.Next != null)
                {
                    if (Head.Next == Rear)
                    {
                        lock (Rear)
                        {
                            if (Head.Next == Rear)
                            {
                                Rear = Head;
                            }
                        }
                    }
                    Head.Next = Head.Next.Next;
                }
                return res;
            }
        }

        private QueueNode Head { get; set; }
        private QueueNode Rear { get; set; }

        private Option<A> InnerPeek() => Option(Head.Next).Map(n => n.Info);

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
