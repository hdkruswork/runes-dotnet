using System;

using static Runes.Options;
using static Runes.Collections.Immutable.Arrays;

namespace Runes.Collections.Immutable
{
    public static class Lists
    {
        public static List<A> List<A>(params A[] values) => ImmutableArray(values).ToList();
    }

    public abstract class List<A> : SeqLike<A, List<A>>
    {
        public static readonly ICollectionBuilder<A, List<A>> Builder = new ListBuilder<A>();

        public static readonly List<A> Empty = new EmptyList<A>();

        public List<B> Collect<B>(IPartialFunction<A, B> pf) => Collect(pf, List<B>.Builder);

        public Option<A> GetAt(int idx)
        {
            var currIdx = idx >= 0 ? 0 : -Size();
            var curr = this;
            while (currIdx < idx && curr.NonEmpty)
            {
                curr = curr.Tail;
                currIdx += 1;
            }
            return curr.HeadOption;
        }

        public List<B> FlatMap<B, That>(Func<A, That> f) where That: ITraversable<B> => FlatMap(f, List<B>.Builder);

        public List<B> Map<B>(Func<A, B> f) => Map(f, List<B>.Builder);

        public override List<A> Prepend(A e) => new ConsList<A>(e, this);

        public List<(A, B)> Zip<B>(List<B> other) => Zip(other, List<(A, B)>.Builder);

        public List<(A, int)> ZipWithIndex() => ZipWithIndex(List<(A, int)>.Builder);

        protected override ICollectionBuilder<A, List<A>> NewBuilder() => Builder;

        // Private members

        private sealed class ListBuilder<T> : CollectionBuilder<T, List<T>>
        {
            public ListBuilder() : base() {}
            public ListBuilder(T rear, CollectionBuilder<T, List<T>> init) : base(rear, init) { }

            public override CollectionBuilder<T, List<T>> NewBuilder() => new ListBuilder<T>();

            public override List<T> Empty() => List<T>.Empty;

            protected override CollectionBuilder<T, List<T>> NewBuilder(T rear, CollectionBuilder<T, List<T>> init) =>
                new ListBuilder<T>(rear, init);
        }
    }

    public sealed class EmptyList<A> : List<A>
    {
        public override Option<A> HeadOption => None<A>();

        public override List<A> Tail => this;

        public override string ToString() => string.Empty;

        internal EmptyList() {}
    }

    internal class ConsList<A> : List<A>
    {
        public ConsList(A head, List<A> tail)
        {
            Head = head;
            Tail = tail;
        }

        public A Head { get; }

        public override Option<A> HeadOption => Some(Head);
        public override List<A> Tail { get; }

        public override string ToString() =>
            $"{Head}{(Tail.NonEmpty ? $", {Tail}" : string.Empty)}";
    }
}
