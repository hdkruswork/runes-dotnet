using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runes.Collections.Immutable;

namespace Runes.Core.Test.Collections.Immutable
{
    [TestClass]
    public class StreamTest
    {
        [TestMethod]
        public void TestFibonacci()
        {
            Stream<int> fibonacci() => Stream.Of(0, 1, Lazy.Of(
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
            Stream<int> factorial() => Stream.Of(1, Lazy.Of(
                () => factorial().Zip(Stream.From(1)).Map(p => p.Item1 * p.Item2)
            ));

            var factStream = factorial();
            
            var expected = Stream.Of(1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880)
                .Correspond(factStream.Take(10), (a, b) => a == b);

            Assert.IsTrue(expected);
        }
    }
}
