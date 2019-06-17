namespace Runes.Collections
{
    public interface IGrowable<A> : ICollection<A, IGrowable<A>>
    {
        bool Accepts(A item);

        IGrowable<A> Append(A item);
        IGrowable<A> Append(IIterable<A> iterable);
        IGrowable<A> Prepend(A item);
        IGrowable<A> Prepend(IIterable<A> iterable);
    }

    public interface IGrowable<A, out CC> : IGrowable<A> where CC : IGrowable<A, CC>
    {
        new CC Append(A item);
        new CC Append(IIterable<A> iterable);
        new CC Prepend(A item);
        new CC Prepend(IIterable<A> iterable);
    }

    public static class GrowableUtils
    {
        public static CC Append<A, CC>(A item, CC iterable, IFactory<A, CC> factory) where CC : IIterable<A, CC> =>
            factory
                .CreateBuilderFrom(iterable)
                .Append(item)
                .Build();

        public static CC Append<A, CC>(IIterable<A> other, CC iterable, IFactory<A, CC> factory)
            where CC : IIterable<A, CC>
        {
            if (other.IsEmpty)
            {
                return iterable;
            }

            if (iterable.IsEmpty)
            {
                return other.To(factory);
            }

            return factory
                .CreateBuilderFrom(iterable, other)
                .Build();
        }

        public static CC Prepend<A, CC>(IIterable<A> other, CC iterable, IFactory<A, CC> factory)
            where CC : IIterable<A, CC>
        {
            if (other.IsEmpty)
            {
                return iterable;
            }

            if (iterable.IsEmpty)
            {
                return other.To(factory);
            }

            return factory
                .CreateBuilderFrom(other, iterable)
                .Build();
        }
    }
}
