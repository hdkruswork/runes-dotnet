using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runes.Collections;

namespace Runes.Test
{
    public static class MonadTest
    {
        public static void TestMonadType<M>(IFactory<int, M> factory) where M : IMonad<int>
        {
            TestLeftIdentityRule(factory);
            TestRightIdentityRule(factory);
            TestAssociativityRule(factory);
        }

        private static void TestLeftIdentityRule<M>(IFactory<int, M> factory) where M : IMonad<int>
        {
            var value = 1;
            var monad = factory.NewBuilder().Append(value).Build();
            IMonad<int> f(int i) => factory.NewBuilder().Append(i * 2).Build();
            Assert.AreEqual(
                monad.FlatMap(f),
                f(value),
                $"Type '{typeof(M)}' doesn't obey the left identity rule of monads"
            );
        }

        private static void TestRightIdentityRule<M>(IFactory<int, M> factory) where M : IMonad<int>
        {
            var value = 1;
            var monad = factory.NewBuilder().Append(value).Build();
            Assert.AreEqual(
                monad,
                monad.FlatMap(v => factory.NewBuilder().Append(v).Build()),
                $"Type '{typeof(M)}' doesn't obey the right identity rule of monads"
            );
        }

        private static void TestAssociativityRule<M>(IFactory<int, M> factory) where M : IMonad<int>
        {
            var value = 1;
            var monad = factory.NewBuilder().Append(value).Build();
            IMonad<int> f(int i) => factory.NewBuilder().Append(i * 2).Build();
            IMonad<int> g(int i) => factory.NewBuilder().Append(i + 6).Build();

            Assert.AreEqual(
                monad.FlatMap(f).FlatMap(g),
                monad.FlatMap(v => f(v).FlatMap(g)),
                $"Type '{typeof(M)}' doesn't obey the associativity rule of monads"
            );
        }
    }
}
