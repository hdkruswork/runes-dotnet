using System;
using System.Collections.Generic;
using Runes.Math;

namespace Runes.Collections
{
    public interface IIterable<A> : ICollection<A>, IEnumerable<A>
    {
        Option<A> HeadOption { get; }
        IIterable<A> Tail { get; }

        IIterable<B> As<B>();
        IIterable<B> Collect<B>(Func<A, Option<B>> f);
        IIterable<A> Drops(Int count);
        IIterable<A> DropsWhile(Func<A, bool> p);
        IIterable<A> DropsWhileNot(Func<A, bool> p);
        IIterable<A> DropsWhile(Func<A, bool> p, out Int dropped);
        IIterable<A> DropsWhileNot(Func<A, bool> p, out Int dropped);
        bool Exists(Func<A, bool> p);
        bool ExistsNot(Func<A, bool> p);
        IIterable<B> FlatMap<B>(Func<A, IIterable<B>> f);
        bool ForAll(Func<A, bool> p);
        That FoldLeft<That>(That initialValue, Func<That, A, That> f);
        That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);
        Unit Foreach(Action<A> action);
        Unit ForeachWhile(Func<A, bool> p, Action<A> action);
        IIterable<B> Map<B>(Func<A, B> f);
        (IIterable<A>, IIterable<A>) Partition(Func<A, bool> p);
        IIterable<A> Take(Int count);
        IIterable<A> TakeWhile(Func<A, bool> p);
        IIterable<A> TakeWhileNot(Func<A, bool> p);
        That To<That>(IFactory<A, That> factory) where That : IIterable<A>;
        Array<A> ToArray();
        List<A> ToList();
        A[] ToMutableArray();
        Stream<A> ToStream();
        (IIterable<X>, IIterable<Y>) Unzip<X, Y>(Func<A, (X, Y)> f);
        IIterable<(A, B)> Zip<B>(IIterable<B> other);
    }

    public interface IIterable<A, CC> :IIterable<A>, ICollection<A, CC> where CC : IIterable<A, CC>
    {
        new CC Tail { get; }

        new CC Drops(Int count);
        new CC DropsWhile(Func<A, bool> p);
        new CC DropsWhileNot(Func<A, bool> p);
        new CC DropsWhile(Func<A, bool> p, out Int dropped);
        new CC DropsWhileNot(Func<A, bool> p, out Int dropped);
        new (CC, CC) Partition(Func<A, bool> p);
        new CC Take(Int count);
        new CC TakeWhile(Func<A, bool> p);
        new CC TakeWhileNot(Func<A, bool> p);
    }
}
