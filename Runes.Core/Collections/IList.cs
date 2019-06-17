using System;
using Runes.Math;

namespace Runes.Collections
{
    public interface IList<A> : IEagerIterable<A>, IGrowable<A>, IIndexable<A>
    {
        new IList<A> Tail { get; }

        new IList<A> Append(A item);
        new IList<A> Append(IIterable<A> iterable);
        new IList<A> Drops(Int count);
        new IList<A> DropsWhile(Func<A, bool> p);
        new IList<A> DropsWhileNot(Func<A, bool> p);
        new IList<A> DropsWhile(Func<A, bool> p, out Int dropped);
        new IList<A> DropsWhileNot(Func<A, bool> p, out Int dropped);
        new IList<A> Filter(Func<A, bool> p);
        new IList<A> FilterNot(Func<A, bool> p);
        new IList<A> Prepend(A item);
        new IList<A> Prepend(IIterable<A> iterable);
        new IList<A> Reverse();
        new IList<A> Sort(Ordering<A> ord);
        new IList<A> Take(Int count);
        new IList<A> TakeWhile(Func<A, bool> p);
        new IList<A> TakeWhileNot(Func<A, bool> p);
        new IList<(A, Int)> ZipWithIndex();
    }

    public interface IList<A, out CC> : IList<A> where CC : IList<A, CC>
    {
        new CC Tail { get; }

        new CC Append(A item);
        new CC Append(IIterable<A> iterable);
        new CC Drops(Int count);
        new CC DropsWhile(Func<A, bool> p);
        new CC DropsWhileNot(Func<A, bool> p);
        new CC DropsWhile(Func<A, bool> p, out Int dropped);
        new CC DropsWhileNot(Func<A, bool> p, out Int dropped);
        new CC Filter(Func<A, bool> p);
        new CC FilterNot(Func<A, bool> p);
        new CC Prepend(A item);
        new CC Prepend(IIterable<A> iterable);
        new CC Reverse();
        new CC Sort(Ordering<A> ord);
        new CC Take(Int count);
        new CC TakeWhile(Func<A, bool> p);
        new CC TakeWhileNot(Func<A, bool> p);
    }
}
