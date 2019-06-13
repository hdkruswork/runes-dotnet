using System;
using Runes.Math;
using static Runes.Predef;

namespace Runes.Collections
{
    public static class Lists
    {
        public static List<A> Empty<A>() => EmptyList<A>.Object;
        public static List<A> Create<A>(A head, List<A> tail) => new NonEmptyList<A>(head, tail);

        private sealed class NonEmptyList<A> : List<A>
        {
            public override Option<A> HeadOption { get; }
            public override List<A> Tail { get; }

            internal NonEmptyList(A head, List<A> tail)
            {
                HeadOption = Some(head);
                Tail = tail;
            }
        }

        private sealed class EmptyList<A> : List<A>
        {
            public static readonly EmptyList<A> Object = new EmptyList<A>();

            public override Option<A> HeadOption => None<A>();
            public override List<A> Tail => this;

            private EmptyList() { }
        }
    }

    public abstract class List<A> : IterableBase<A, List<A>>, IList<A, List<A>>
    {
        public static ListBuilder CreateListBuilder() => new ListBuilder();

        public List<B> As<B>() where B : class=>
            As<B, List<B>, List<B>.ListBuilder>(List<B>.CreateListBuilder());

        public List<A> Append(A item)
        {
            var builder = CreateListBuilder();
            Foreach(it => builder.Add(it));
            builder.Add(item);

            return builder.Build();
        }

        public List<A> Append(IIterable<A> iterable) => HeadOption.GetIfPresent(out A head)
            ? List(head, Tail.Append(iterable))
            : iterable.ToList();

        public List<B> Collect<B>(Func<A, Option<B>> f) =>
            Collect<B, List<B>, List<B>.ListBuilder>(f, List<B>.CreateListBuilder());

        public override List<A> Filter(Func<A, bool> p) =>
            Filter(p, true, CreateListBuilder());

        public override List<A> FilterNot(Func<A, bool> p) =>
            Filter(p, false, CreateListBuilder());

        public List<B> FlatMap<B>(Func<A, ICollection<B>> f) =>
            FlatMap<B, List<B>, List<B>.ListBuilder>(f, List<B>.CreateListBuilder());

        public List<B> Map<B>(Func<A, B> f) =>
            Map<B, List<B>, List<B>.ListBuilder>(f, List<B>.CreateListBuilder());

        public List<A> Prepend(A item) =>
            List(item, this);

        public List<A> Prepend(IIterable<A> iterable)
        {
            var reversed = EmptyList<A>();
            iterable.Foreach(it => reversed = reversed.Prepend(it));
            
            var res = this;
            reversed.Foreach(it => res = res.Prepend(it));
            return res;
        }

        public override List<A> Reversed() =>
            FoldLeft(EmptyList<A>(), (reversed, it) => reversed.Prepend(it));

        public override List<A> Sort(Ordering<A> ord) => this.ToMArray().Sort(ord).ToList();

        public override List<A> Take(int count) => Take(count, CreateListBuilder());

        public override List<A> TakeWhile(Func<A, bool> p) => TakeWhile(p, true, CreateListBuilder());

        public override List<A> TakeWhileNot(Func<A, bool> p) => TakeWhile(p, false, CreateListBuilder());

        public (List<X>, List<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) =>
            Unzip<X, Y, List<X>, List<Y>, List<X>.ListBuilder, List<Y>.ListBuilder>(
                toPairFunc,
                List<X>.CreateListBuilder(),
                List<Y>.CreateListBuilder()
            );

        public List<(A, B)> Zip<B>(ICollection<B> other) =>
            Zip<B, List<(A, B)>, List<(A, B)>.ListBuilder>(other, List<(A, B)>.CreateListBuilder());

        public List<(A, int)> ZipWithIndex() =>
            ZipWithIndex<List<(A, int)>, List<(A, int)>.ListBuilder>(List<(A, int)>.CreateListBuilder());

        // protected members

        protected override IIterable<B> IterableAs<B>() => As<B>();
        protected override IIterable<B> IterableCollect<B>(Func<A, Option<B>> f) => Collect(f);
        protected override IIterable<B> IterableFlatMap<B>(Func<A, ICollection<B>> f) => FlatMap(f);
        protected override IIterable<B> IterableMap<B>(Func<A, B> f) => Map(f);
        protected override (IIterable<X>, IIterable<Y>) IterableUnzip<X, Y>(Func<A, (X, Y)> toPairFunc) => Unzip(toPairFunc);
        protected override IIterable<(A, B)> IterableZip<B>(ICollection<B> other) => Zip(other);
        protected override IIterable<(A, int)> IterableZipWithIndex() => ZipWithIndex();

        // private members

        IList<A> IList<A>.Tail => Tail;

        IList<B> IList<A>.As<B>() => As<B>();
        IGrowable<A> IGrowable<A>.Append(A item) => Append(item);
        IGrowable<A> IGrowable<A>.Append(IIterable<A> iterable) => Append(iterable);
        IList<B> IList<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f);
        IList<B> IList<A>.FlatMap<B>(Func<A, ICollection<B>> f) => FlatMap(f);
        IList<B> IList<A>.Map<B>(Func<A, B> f) => Map(f);
        IGrowable<A> IGrowable<A>.Prepend(A item) => Prepend(item);
        IGrowable<A> IGrowable<A>.Prepend(IIterable<A> iterable) => Prepend(iterable);
        (IList<X>, IList<Y>) IList<A>.Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) => Unzip(toPairFunc);
        IList<(A, B)> IList<A>.Zip<B>(ICollection<B> other) => Zip(other);
        IList<(A, int)> IList<A>.ZipWithIndex() => ZipWithIndex();

        public class ListBuilder : ICollectionBuilder<A, List<A>>
        {
            private List<A> reversed = EmptyList<A>();

            public void Add(A item) => reversed = reversed.Prepend(item);

            public List<A> Build() => reversed.Reversed();
        }
    }
}
