using System;
using System.Collections.Generic;

namespace Runes.Collections
{
    public interface ITraversable<A> : ICollection<A>, IEnumerable<A>
    {
        new ITraversable<A> Tail { get; }
        long Size { get; }

        bool ForAll(Func<A, bool> p);
        That FoldLeft<That>(That initialValue, Func<That, A, That> f);
        That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);
        Unit Foreach(Action<A> action);
        Unit ForeachWhile(Func<A, bool> p, Action<A> action);

        new ITraversable<B> As<B>() where B : class;
        new ITraversable<B> Collect<B>(Func<A, Option<B>> f);
        new ITraversable<B> FlatMap<B>(Func<A, ICollection<B>> f);
        new ITraversable<B> Map<B>(Func<A, B> f);
        ITraversable<A> Reversed();
        new (ITraversable<X>, ITraversable<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        new ITraversable<(A, B)> Zip<B>(ICollection<B> other);
        new ITraversable<(A, int)> ZipWithIndex();
    }

    public interface ITraversable<A, CC> :ITraversable<A>, ICollection<A, CC> where CC : ITraversable<A, CC>
    {
        new CC Reversed();
    }
}
