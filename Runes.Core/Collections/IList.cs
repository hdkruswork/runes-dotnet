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
        IList<IArray<A>> Slice(int size, int nextStep);
        new IList<A> Sort(Ordering<A> ord);
        new IList<A> Take(Int count);
        new IList<A> TakeWhile(Func<A, bool> p);
        new IList<A> TakeWhileNot(Func<A, bool> p);
        new IList<(A, Int)> ZipWithIndex();

        // explicit definitions

        IIndexable<A> IIndexable<A>.Tail => Tail;

        IIndexable<(A, Int)> IIndexable<A>.ZipWithIndex() => ZipWithIndex();
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

        // explicit definitions

        IList<A> IList<A>.Tail => Tail;

        IList<A> IList<A>.Append(A item) => Append(item);
        IList<A> IList<A>.Append(IIterable<A> iterable) => Append(iterable);
        IList<A> IList<A>.Drops(Int count) => Drops(count);
        IList<A> IList<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);
        IList<A> IList<A>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);
        IList<A> IList<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);
        IList<A> IList<A>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);
        IList<A> IList<A>.Filter(Func<A, bool> p) => Filter(p);
        IList<A> IList<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
        IList<A> IList<A>.Prepend(A item) => Prepend(item);
        IList<A> IList<A>.Prepend(IIterable<A> iterable) => Prepend(iterable);
        IList<A> IList<A>.Reverse() => Reverse();
        IList<A> IList<A>.Sort(Ordering<A> ord) => Sort(ord);
        IList<A> IList<A>.Take(Int count) => Take(count);
        IList<A> IList<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);
        IList<A> IList<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);
    }
}
