using System;
using Runes.Math;

namespace Runes.Collections
{
    public interface IArray<A> : IEagerIterable<A>, IIndexable<A>
    {
        Option<A> this[long index] { get; }
        Option<A> this[Index index] { get; }

        long Length { get; }

        new IArray<A> Tail { get; }
        IArray<A> Init { get; }
        Option<A> Rear { get; }

        That FoldRight<That>(That initialValue, Func<That, A, That> f);
        That FoldRightWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);
        (IArray<A>, IArray<A>) SplitAt(long idx);
        IIterable<IArray<A>> Slide(long size, long nextStep);

        new IArray<(A, Int)> ZipWithIndex();
    }

    public interface IArray<A, CC> : IArray<A>, IEagerIterable<A, CC> where CC : IArray<A, CC>
    {
        new CC Tail { get; }
        new CC Init { get; }

        new (CC, CC) SplitAt(long idx);
        new IIterable<CC> Slide(long size, long nextStep);
    }
}
