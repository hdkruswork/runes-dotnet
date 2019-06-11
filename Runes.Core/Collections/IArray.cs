using System;

namespace Runes.Collections
{
    public interface IArray<A> : ITraversable<A>
    {
        Option<A> this[long index] { get; }

        IArray<A> Init { get; }
        Option<A> Rear { get; }

        long Length { get; }

        new IArray<B> As<B>() where B : class;
        new IArray<B> Collect<B>(Func<A, Option<B>> f);
        new IArray<B> FlatMap<B>(Func<A, ICollection<B>> f);
        Unit For(Action<A, long> action);
        new IArray<B> Map<B>(Func<A, B> f);
        new(IArray<X>, IArray<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        new IArray<(A, B)> Zip<B>(ICollection<B> other);
        new IArray<(A, int)> ZipWithIndex();

        That FoldRight<That>(That initialValue, Func<That, A, That> f);
        That FoldRightWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);
        (IArray<A>, IArray<A>) Split(long index);
        ITraversable<IArray<A>> Sliding(int size, int step = 1);
    }

    public interface IArray<A, CC> : IArray<A>, ITraversable<A, CC> where CC : IArray<A, CC>
    {
        new CC Init { get; }

        new (CC, CC) Split(long index);
        new ITraversable<CC> Sliding(int size, int step = 1);
    }
}
