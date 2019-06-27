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
    }
}
