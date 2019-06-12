namespace Runes.Collections
{
    public interface IGrowable<A>
    {
        IGrowable<A> Append(A item);
        IGrowable<A> Append(IIterable<A> iterable);
        IGrowable<A> Prepend(A item);
        IGrowable<A> Prepend(IIterable<A> iterable);
    }

    public interface IGrowable<A, out GG> : IGrowable<A> where GG : IGrowable<A, GG>
    {
        new GG Append(A item);
        new GG Append(IIterable<A> iterable);
        new GG Prepend(A item);
        new GG Prepend(IIterable<A> iterable);
    }
}
