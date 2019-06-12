using System;

namespace Runes.Collections
{
    public interface IStream<A> : ICollection<A>, IGrowable<A>
    {
        new IStream<A> Tail { get; }

        new IStream<A> Append(A item);
        new IStream<A> Append(IIterable<A> item);
        new IStream<B> Collect<B>(Func<A, Option<B>> f);
        new IStream<B> Map<B>(Func<A, B> f);
        new IStream<A> Prepend(A item);
        new IStream<A> Prepend(IIterable<A> item);
        new IStream<(A, B)> Zip<B>(ICollection<B> other);
        new IStream<(A, int)> ZipWithIndex();

        IStream<IStream<A>> Sliding(int size, int step);
    }
    public interface IStream<A, out CC> : IStream<A>, ICollection<A, CC>, IGrowable<A, CC> where CC : IStream<A, CC>
    {
        new CC Tail { get; }
    }
}
