using System;
using Runes.Math;

using static Runes.Predef;

namespace Runes.Collections
{
    public abstract class List<A> : EagerGrowableBase<A, List<A>>, IList<A, List<A>>
    {
        public static readonly List<A> Empty = new EmptyList();

        public static readonly IFactory<A, List<A>> Factory = new ListFactory();

        public static List<A> Create(A head, List<A> tail) => new NonEmptyList(head, tail);

        public List<B> As<B>() => As(List<B>.Factory);

        public List<B> Collect<B>(Func<A, Option<B>> f) => Collect(f, List<B>.Factory);

        public List<B> FlatMap<B>(Func<A, IIterable<B>> f) => FlatMap(f, List<B>.Factory);

        public Option<A> GetAt(Int idx) => Drops(idx).HeadOption;

        public List<B> Map<B>(Func<A, B> f) => Map(f, List<B>.Factory);

        public override List<A> Prepend(A item) => Create(item, this);

        public List<Array<A>> Slice(int size, int nextStep) =>
            CollectionHelper.Slice(this, Array<A>.Factory, List<Array<A>>.Factory, size, nextStep);

        public (List<X>, List<Y>) Unzip<X, Y>(Func<A, (X, Y)> f) => Unzip(f, List<X>.Factory, List<Y>.Factory);

        public List<(A, B)> Zip<B>(IIterable<B> other) => Zip(other, List<(A, B)>.Factory);

        public List<(A, Int)> ZipWithIndex()
        {
            var currentThis = This;
            Int index = 0;
            var builder = List<(A, Int)>.Factory.NewBuilder();
            while (currentThis.HeadOption.GetIfPresent(out var a))
            {
                builder.Append((a, index));
                currentThis = currentThis.Tail;
                index += 1;
            }

            return builder.Build();
        }

        // protected members

        protected override IFactory<B, IIterable<B>> GetFactory<B>() => List<B>.Factory;

        protected override IFactory<A, List<A>> GetFactory() => Factory;

        // explicit definitions

        IList<IArray<A>> IList<A>.Slice(int size, int nextStep) => Slice(size, nextStep).As<IArray<A>>();

        IList<(A, Int)> IList<A>.ZipWithIndex() => ZipWithIndex();

        // inner types

        private sealed class NonEmptyList : List<A>
        {
            public NonEmptyList(A head, List<A> tail)
            {
                HeadOption = Some(head);
                Tail = tail;
            }

            public override Option<A> HeadOption { get; }

            public override List<A> Tail { get; }
        }

        private sealed class EmptyList : List<A>
        {
            public override Option<A> HeadOption => None<A>();

            public override List<A> Tail => this;
        }

        private sealed class ListFactory : FactoryBase<A, List<A>>
        {
            public override IIterableBuilder<A, List<A>> NewBuilder() => new Builder();
        }

        private sealed class Builder : Builder<Builder>
        {
            private List<A> innerList = Empty;

            public override Builder Append(A item)
            {
                innerList = innerList.Prepend(item);
                return this;
            }

            public override List<A> Build() => innerList.Reverse();

            public override List<A> GetEmpty() => Empty;
        }
    }
}
