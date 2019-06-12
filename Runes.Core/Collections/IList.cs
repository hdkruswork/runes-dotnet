using System;

namespace Runes.Collections
{
    public interface IList<A> : IIterable<A>, IGrowable<A>
    {
        new IList<A> Tail { get; }

        new IList<B> As<B>() where B : class;
        new IList<B> Collect<B>(Func<A, Option<B>> f);
        new IList<B> FlatMap<B>(Func<A, ICollection<B>> f);
        new IList<B> Map<B>(Func<A, B> f);
        new (IList<X>, IList<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        new IList<(A, B)> Zip<B>(ICollection<B> other);
        new IList<(A, int)> ZipWithIndex();
    }

    public interface IList<A, out CC> : IList<A>, IIterable<A, CC>, IGrowable<A, CC> where CC : IList<A, CC>
    {
        new CC Tail { get;  }
    }
}
