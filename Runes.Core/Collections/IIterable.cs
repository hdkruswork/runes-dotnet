using System;
using System.Collections.Generic;
using Runes.Math;

namespace Runes.Collections
{
    public interface IIterable<A> : ICollection<A>, IEnumerable<A>
    {
        new IIterable<A> Tail { get; }
        long Size { get; }

        bool ForAll(Func<A, bool> p);
        That FoldLeft<That>(That initialValue, Func<That, A, That> f);
        That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);
        Unit Foreach(Action<A> action);
        Unit ForeachWhile(Func<A, bool> p, Action<A> action);

        new IIterable<B> As<B>() where B : class;
        new IIterable<B> Collect<B>(Func<A, Option<B>> f);
        new IIterable<B> FlatMap<B>(Func<A, ICollection<B>> f);
        new IIterable<B> Map<B>(Func<A, B> f);
        IIterable<A> Reversed();
        IIterable<A> Sort(Ordering<A> ord);
        new (IIterable<X>, IIterable<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        new IIterable<(A, B)> Zip<B>(ICollection<B> other);
        new IIterable<(A, int)> ZipWithIndex();
    }

    public interface IIterable<A, out CC> :IIterable<A>, ICollection<A, CC> where CC : IIterable<A, CC>
    {
        new CC Tail { get; }

        new CC Reversed();
        new CC Sort(Ordering<A> ord);
    }
}
