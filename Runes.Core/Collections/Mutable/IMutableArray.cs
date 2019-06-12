namespace Runes.Collections.Mutable
{
    public interface IMutableArray<A> : IArray<A>
    {
        new A this[long index] { get; set; }

        new IMutableArray<A> Tail { get; }
        new IMutableArray<A> Init { get; }

        new IMutableArray<A> Reversed();
        new (IMutableArray<A>, IMutableArray<A>) Split(long index);
        new IIterable<IMutableArray<A>> Sliding(int size, int step = 1);
    }

    public interface IMutableArray<A, CC> : IMutableArray<A>, IArray<A, CC> where CC : IMutableArray<A, CC>
    {
        new CC Init { get; }
        new CC Tail { get; }
    }
}
