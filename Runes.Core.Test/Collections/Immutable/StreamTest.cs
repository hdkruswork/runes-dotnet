using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Runes.LazyExtensions;

namespace Runes.Collections.Immutable.Test
{
    [TestClass]
    public class StreamTest
    {
        [TestMethod]
        public void TestFibonacci()
        {
            Stream<int> fibonacci() => Stream.Of(0, 1, Lazy(
                () => fibonacci().Zip(fibonacci().Tail).Map(p => p.Item1 + p.Item2)
            ));
            var fibStream = fibonacci();

            var expected = Stream.Of(0, 1, 1, 2, 3, 5, 8, 13, 21, 34)
                .Correspond(fibStream.Take(10), (a, b) => a == b);

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestFactorial()
        {
            Stream<int> factorial() => Stream.Of(1, Lazy(
                () => factorial().Zip(Stream.From(1)).Map(p => p.Item1 * p.Item2)
            ));

            var factStream = factorial();
            
            var expected = Stream.Of(1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880)
                .Correspond(factStream.Take(10), (a, b) => a == b);

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestSliding()
        {
            var pairs = Stream.Of(1, 2, 3, 4, 5, 6)
                .Sliding(2, 2)
                .Collect(arr => (arr[0], arr[1]))
                .ToArray();

            Assert.AreEqual((1, 2), pairs[0]);
            Assert.AreEqual((3, 4), pairs[1]);
            Assert.AreEqual((5, 6), pairs[2]);
        }
    }
}
