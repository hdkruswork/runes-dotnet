using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runes.Collections.Mutable.Test
{
    [TestClass]
    public class QueueTest
    {
        [TestMethod]
        public void TestEmptyInvariants()
        {
            var queue = new Queue<int>();
            Assert.IsTrue(queue.IsEmpty);
            Assert.IsFalse(queue.NonEmpty);
            Assert.IsFalse(queue.Peek(out _));
            Assert.IsFalse(queue.Dequeue(out _));
        }

        [TestMethod]
        public void TestEnqueue()
        {
            var queue = new Queue<int>();
            queue.Enqueue(10);
            Assert.IsTrue(queue.NonEmpty);
            Assert.IsFalse(queue.IsEmpty);
            Assert.IsTrue(queue.Peek(out var first));
            Assert.AreEqual(10, first);
        }

        [TestMethod]
        public void TestDequeue()
        {
            var queue = new Queue<int>();
            queue.Enqueue(10);
            queue.Dequeue(out var first);
            Assert.IsTrue(queue.IsEmpty);
            Assert.IsFalse(queue.NonEmpty);
            Assert.IsFalse(queue.Peek(out _));
            Assert.AreEqual(10, first);
        }
    }
}
