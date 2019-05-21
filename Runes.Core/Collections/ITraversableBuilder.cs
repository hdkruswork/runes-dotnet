using Runes.Collections.Immutable;

namespace Runes.Collections
{
    public static class Builders
    {
        public static ITraversableBuilder<A, List<A>> ListBuilder<A>() => new ListBuilder<A>();
    }

    public interface ITraversableBuilder<A, T> : IBuilder<T> where T : ITraversable<A>
    {
        ITraversableBuilder<A, T> Append(A item);
        ITraversableBuilder<A, T> Append(ITraversable<A> traversable);
    }

    public abstract class TraversableBuilder<A, T> : ITraversableBuilder<A, T> where T : ITraversable<A>
    {
        protected Stream<A> stream = Streams.Empty<A>();

        public virtual ITraversableBuilder<A, T> Append(A item)
        {
            stream = stream.Append(item);

            return this;
        }
        public virtual ITraversableBuilder<A, T> Append(ITraversable<A> traversable)
        {
            traversable.Foreach(item =>
            {
                stream = stream.Append(item);
            });

            return this;
        }

        public abstract T Build();
    }
}
