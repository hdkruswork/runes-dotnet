using System.Collections.Generic;

namespace Runes.Collections
{
    public interface IFactory<A, out CC> where CC : IIterable<A>
    {
        CC From(IEnumerable<A> e);

        IIterableBuilder<A, CC> CreateBuilderFrom(params IEnumerable<A>[] iterables);

        IIterableBuilder<A, CC> NewBuilder();
    }

    public abstract class FactoryBase<A, CC> : IFactory<A, CC> where CC : IIterable<A>
    {
        public virtual CC From(IEnumerable<A> e)
        {
            if (e is CC cc)
            {
                return cc;
            }

            return CreateBuilderFrom(e).Build();
        }

        public IIterableBuilder<A, CC> CreateBuilderFrom(params IEnumerable<A>[] iterables)
        {
            var builder = NewBuilder();

            foreach (var iterable in iterables)
            {
                foreach (var item in iterable)
                {
                    builder.Append(item);
                }
            }

            return builder;
        }

        public abstract IIterableBuilder<A, CC> NewBuilder();
    }
}
