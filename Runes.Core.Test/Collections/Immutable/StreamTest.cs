using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Runes.Collections.Immutable.Streams;
using static Runes.Lazies;

namespace Runes.Collections.Immutable.Test
{
    [TestClass]
    public class StreamTest
    {
        [TestMethod]
        public void TestFibonacci()
        {
            Stream<int> fibonacci() => Stream(0, 1, Lazy(
                () => fibonacci().Zip(fibonacci().Tail).Map(p => p.Item1 + p.Item2)
            ));
            var fibStream = fibonacci();

            var expected = Stream(0, 1, 1, 2, 3, 5, 8, 13, 21, 34)
                .Correspond(fibStream.Take(10), (a, b) => a == b);

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestFactorial()
        {
            Stream<int> factorial() => Stream(1, Lazy(
                () => factorial().Zip(Streams.StartStream(1)).Map(p => p.Item1 * p.Item2)
            ));

            var factStream = factorial();
            
            var expected = Stream(1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880)
                .Correspond(factStream.Take(10), (a, b) => a == b);

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestSliding()
        {
            var pairs = Stream(1, 2, 3, 4, 5, 6)
                .Sliding(2, 2)
                .Collect(arr => (arr[0], arr[1]))
                .ToArray();

            Assert.AreEqual((1, 2), pairs[0]);
            Assert.AreEqual((3, 4), pairs[1]);
            Assert.AreEqual((5, 6), pairs[2]);
        }
    }
}
