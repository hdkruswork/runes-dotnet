namespace Runes.Collections
{
    public interface IGrowable<A>
    {
        IGrowable<A> Append(A item);
        IGrowable<A> Append(ITraversable<A> traversable);
        IGrowable<A> Prepend(A item);
        IGrowable<A> Prepend(ITraversable<A> traversable);
    }

    public interface IGrowable<A, GG> : IGrowable<A> where GG : IGrowable<A, GG>
    {
        new GG Append(A item);
        new GG Append(ITraversable<A> traversable);
        new GG Prepend(A item);
        new GG Prepend(ITraversable<A> traversable);
    }
}
