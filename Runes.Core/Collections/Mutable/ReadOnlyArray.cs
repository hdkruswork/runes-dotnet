namespace Runes.Collections.Mutable
{
    public sealed class ReadOnlyArray<A> : Array<A>
    {
        internal ReadOnlyArray(A[] array) : base(array, 0, array.LongLength, 1)
        {
        }
    }
}
