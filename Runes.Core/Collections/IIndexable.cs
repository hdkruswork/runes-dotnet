using Runes.Math;

namespace Runes.Collections
{
    public interface IIndexable<A> : IIterable<A>
    {
        new IIndexable<A> Tail { get; }

        Option<A> GetAt(Int idx);

        IIndexable<(A, Int)> ZipWithIndex();
    }

    public static class IndexableUtil
    {
        public static CC ZipWithIndex<A, CC>(IIndexable<A> array, IFactory<(A, Int), CC> factory)
            where CC : IIterable<(A, Int)>
        {
            var currentThis = array;
            Int index = 0;
            var builder = factory.NewBuilder();
            while (currentThis.HeadOption.GetIfPresent(out var a))
            {
                builder.Append((a, index));
                currentThis = currentThis.Tail;
                index += 1;
            }

            return builder.Build();
        }
    }
}
