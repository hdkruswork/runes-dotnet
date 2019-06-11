namespace Runes.Collections
{
    public interface IMutableArray<A> : IArray<A>
    {
        new A this[long index] { get; set; }

        new IMutableArray<A> Tail { get; }
        new IMutableArray<A> Init { get; }

        new IMutableArray<A> Reversed();
        new (IMutableArray<A>, IMutableArray<A>) Split(long index);
        new ITraversable<IMutableArray<A>> Sliding(int size, int step = 1);
    }

    public interface IMutableArray<A, CC> : IMutableArray<A>, IArray<A, CC> where CC : IMutableArray<A, CC>
    {
    }
}
