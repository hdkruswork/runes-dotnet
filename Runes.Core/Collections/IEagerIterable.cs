using Runes.Math;

namespace Runes.Collections
{
    public interface IEagerIterable<A> : IIterable<A>
    {
        Int Size { get; }

        IEagerIterable<A> Reverse();
        IEagerIterable<A> Sort(Ordering<A> ord);
    }

    public interface IEagerIterable<A, CC> : IEagerIterable<A>, IIterable<A, CC>
        where CC : IEagerIterable<A, CC>
    {
        new CC Reverse();
        new CC Sort(Ordering<A> ord);

        IEagerIterable<A> IEagerIterable<A>.Reverse() => Reverse();
        IEagerIterable<A> IEagerIterable<A>.Sort(Ordering<A> ord) => Sort(ord);
    }
}
