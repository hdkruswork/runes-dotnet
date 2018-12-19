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
            Assert.IsTrue(queue.Peek().IsEmpty);
            Assert.IsTrue(queue.Dequeue().IsEmpty);
        }

        [TestMethod]
        public void TestEnqueue()
        {
            var queue = new Queue<int>();
            queue.Enqueue(10);
            Assert.IsTrue(queue.NonEmpty);
            Assert.IsFalse(queue.IsEmpty);
            Assert.IsTrue(queue.Peek().NonEmpty);
            Assert.AreEqual(10, queue.Peek().GetOrElse(0));
        }

        [TestMethod]
        public void TestDequeue()
        {
            var queue = new Queue<int>();
            queue.Enqueue(10);
            var opt = queue.Dequeue();
            Assert.IsTrue(queue.IsEmpty);
            Assert.IsFalse(queue.NonEmpty);
            Assert.IsTrue(queue.Peek().IsEmpty);
            Assert.AreEqual(10, opt.GetOrElse(0));
        }
    }
}
