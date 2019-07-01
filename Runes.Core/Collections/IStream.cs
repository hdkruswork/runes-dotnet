using System;
using Runes.Math;

namespace Runes.Collections
{
    public interface IStream<A> : IGrowable<A>, IIndexable<A>
    {
        new IStream<A> Tail { get; }

        new IStream<A> Append(A item);
        new IStream<A> Append(IIterable<A> iterable);
        IStream<A> Append(Func<IStream<A>> streamFunc);
        new IStream<B> Collect<B>(Func<A, Option<B>> f);
        new IStream<A> Drops(Int count);
        new IStream<A> DropsWhile(Func<A, bool> p);
        new IStream<A> DropsWhileNot(Func<A, bool> p);
        new IStream<A> DropsWhile(Func<A, bool> p, out Int dropped);
        new IStream<A> DropsWhileNot(Func<A, bool> p, out Int dropped);
        new IStream<A> Filter(Func<A, bool> p);
        new IStream<A> FilterNot(Func<A, bool> p);
        new IStream<A> Prepend(A item);
        new IStream<A> Prepend(IIterable<A> iterable);
        IStream<IArray<A>> Slice(int size, int nextStep);
        new IStream<A> Take(Int count);
        new IStream<A> TakeWhile(Func<A, bool> p);
        new IStream<A> TakeWhileNot(Func<A, bool> p);
        new (IStream<X>, IStream<Y>) Unzip<X, Y>(Func<A, (X, Y)> f);
        new IStream<(A, B)> Zip<B>(IIterable<B> other);
        new IStream<(A, Int)> ZipWithIndex();

        // explicit definitions

        IIndexable<A> IIndexable<A>.Tail => Tail;

        IIterable<B> IIterable<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f);

        bool IGrowable<A>.Accepts(A item) => true;
        IGrowable<A> IGrowable<A>.Append(A item) => Append(item);
        IGrowable<A> IGrowable<A>.Append(IIterable<A> iterable) => Append(iterable);
        IGrowable<A> IGrowable<A>.Prepend(A item) => Prepend(item);
        IGrowable<A> IGrowable<A>.Prepend(IIterable<A> iterable) => Prepend(iterable);

        IGrowable<A> ICollection<A, IGrowable<A>>.Filter(Func<A, bool> p) => Filter(p);
        IGrowable<A> ICollection<A, IGrowable<A>>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ICollection ICollection.Filter(Func<object, bool> p) => Filter((A a) => p(a));
        ICollection ICollection.FilterNot(Func<object, bool> p) => FilterNot((A a) => p(a));

        IIndexable<(A, Int)> IIndexable<A>.ZipWithIndex() => ZipWithIndex();
    }

    public interface IStream<A, CC> : IStream<A> where CC : IStream<A, CC>
    {
        new CC Tail { get; }

        new CC Append(A item);
        new CC Append(IIterable<A> iterable);
        CC Append(Func<CC> streamFunc);
        new CC Drops(Int count);
        new CC DropsWhile(Func<A, bool> p);
        new CC DropsWhileNot(Func<A, bool> p);
        new CC DropsWhile(Func<A, bool> p, out Int dropped);
        new CC DropsWhileNot(Func<A, bool> p, out Int dropped);
        new CC Filter(Func<A, bool> p);
        new CC FilterNot(Func<A, bool> p);
        new CC Prepend(A item);
        new CC Prepend(IIterable<A> iterable);
        new CC Take(Int count);
        new CC TakeWhile(Func<A, bool> p);
        new CC TakeWhileNot(Func<A, bool> p);

        // explicit definitions

        IStream<A> IStream<A>.Tail => Tail;

        IStream<A> IStream<A>.Append(A item) => Append(item);
        IStream<A> IStream<A>.Append(IIterable<A> iterable) => Append(iterable);
        IStream<A> IStream<A>.Append(Func<IStream<A>> streamFunc) => Append(() => (CC) streamFunc());
        IStream<A> IStream<A>.Drops(Int count) => Drops(count);
        IStream<A> IStream<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);
        IStream<A> IStream<A>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);
        IStream<A> IStream<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);
        IStream<A> IStream<A>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);
        IStream<A> IStream<A>.Filter(Func<A, bool> p) => Filter(p);
        IStream<A> IStream<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
        IStream<A> IStream<A>.Prepend(A item) => Prepend(item);
        IStream<A> IStream<A>.Prepend(IIterable<A> iterable) => Prepend(iterable);
        IStream<A> IStream<A>.Take(Int count) => Take(count);
        IStream<A> IStream<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);
        IStream<A> IStream<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);
    }
}
