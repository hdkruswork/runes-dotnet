using System;

using static Runes.Predef;

namespace Runes.Collections
{
    public abstract class Stream<A> : CollectionBase<A, Stream<A>>, IStream<A, Stream<A>>
    {
        public static readonly Stream<A> Empty = new EmptyStream();

        public Stream<B> As<B>() where B: class =>
            HeadOption.GetIfPresent(out A a) && a.As<B>().GetIfPresent(out B b)
                ? Stream(b, () => Tail.As<B>())
                : EmptyStream<B>();
        public Stream<A> Append(A item) => Append(() => Stream(item));
        public Stream<A> Append(ITraversable<A> traversable) => Append(() => traversable.ToStream());
        public Stream<A> Append(Func<Stream<A>> tailFunc) =>
            HeadOption.GetIfPresent(out A head)
                ? Stream(head, () => Tail.Append(tailFunc))
                : tailFunc();
        public Stream<B> Collect<B>(Func<A, Option<B>> f) => FlatMap(f);
        public override Stream<A> Drops(int count)
        {
            var curr = this;
            var remaining = count;
            while (remaining > 0 && curr.NonEmpty)
            {
                curr = curr.Tail;
                remaining -= 1;
            }

            return curr;
        }
        public override Stream<A> DropsWhile(Func<A, bool> p) => DropsWhile(p, out _, true);
        public override Stream<A> DropsWhile(Func<A, bool> p, out int skipped) => DropsWhile(p, out skipped, true);
        public override Stream<A> DropsWhileNot(Func<A, bool> p) => DropsWhile(p, out _, false);
        public override Stream<A> DropsWhileNot(Func<A, bool> p, out int skipped) => DropsWhile(p, out skipped, false);
        public Stream<B> FlatMap<B>(Func<A, IMonad<B>> f)
        {
            if (HeadOption.GetIfPresent(out A head))
            {
                var stream = f(head)
                    .Map(b => Stream(b))
                    .GetOrElse(EmptyStream<B>());

                return stream.Append(() => Tail.FlatMap(f));
            }

            return EmptyStream<B>();
        }
        public Stream<B> FlatMap<B>(Func<A, ICollection<B>> f)
        {
            if (HeadOption.GetIfPresent(out A head))
            {
                var stream = f(head).ToStream();
                return stream.Append(() => Tail.FlatMap(f));
            }

            return EmptyStream<B>();
        }
        public override Stream<A> Filter(Func<A, bool> p) => Filter(p, true);
        public override Stream<A> FilterNot(Func<A, bool> p) => Filter(p, false);
        public Stream<B> Map<B>(Func<A, B> f) => 
            HeadOption.GetIfPresent(out A head)
                ? Stream(f(head), () => Tail.Map(f))
                : EmptyStream<B>();
        public Stream<A> Prepend(A item) => Stream(item, this);
        public Stream<A> Prepend(ITraversable<A> traversable) => traversable.ToStream().Append(() => this);
        public Stream<Stream<A>> Sliding(int size, int step) => Stream(Take(size), () => Drops(step).Sliding(size, step));
        public override Stream<A> Take(int count)
        {
            if (count <= 0 || !HeadOption.GetIfPresent(out A head))
            {
                return EmptyStream<A>();
            }

            return Stream(head, () => Tail.Take(count - 1));
        }
        public override Stream<A> TakeWhile(Func<A, bool> p) => TakeWhile(p, true);
        public override Stream<A> TakeWhileNot(Func<A, bool> p) => TakeWhile(p, false);
        public (Stream<X>, Stream<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc)
        {
            var zipped = Map(toPairFunc);

            return (zipped.Map(pair => pair.Item1), zipped.Map(pair => pair.Item2));
        }
        public Stream<(A, B)> Zip<B>(ICollection<B> other)
        {
            if (HeadOption.GetIfPresent(out A a) && other.HeadOption.GetIfPresent(out B b))
            {
                return Stream((a, b), () => Tail.Zip(other.Tail));
            }

            return EmptyStream<(A, B)>();
        }
        public Stream<(A, int)> ZipWithIndex() => Zip(StartStream(start: 0).Map(bi => (int)bi));

        public void Deconstruct(out A head, out Stream<A> tail)
        {
            head = HeadOption.GetOrDefault();
            tail = Tail;
        }
        public void Deconstruct(out A first, out A second, out Stream<A> tail)
        {
            first = HeadOption.GetOrDefault();
            second = Tail.HeadOption.GetOrDefault();
            tail = Tail.Tail;
        }

        // protected members

        protected override ICollection<B> CollectionAs<B>() => As<B>();
        protected override ICollection<B> CollectionCollect<B>(Func<A, Option<B>> f) => Collect(f);
        protected override ICollection<B> CollectionFlatMap<B>(Func<A, ICollection<B>> f) => FlatMap(f);
        protected override ICollection<B> CollectionMap<B>(Func<A, B> f) => Map(f);
        protected override (ICollection<X>, ICollection<Y>) CollectionUnzip<X, Y>(Func<A, (X, Y)> toPairFunc) => Unzip(toPairFunc);
        protected override ICollection<(A, B)> CollectionZip<B>(ICollection<B> other) => Zip(other);
        protected override ICollection<(A, int)> CollectionZipWithIndex() => ZipWithIndex();

        // private members

        private Stream() { }

        private Stream<A> DropsWhile(Func<A, bool> p, out int skipped, bool isTruthly)
        {
            var curr = this;
            skipped = 0;
            while (curr.HeadOption.GetIfPresent(out A a) && p(a) == isTruthly)
            {
                curr = curr.Tail;
                skipped += 1;
            }

            return curr;
        }
        private Stream<A> Filter(Func<A, bool> p, bool isTruthly)
        {
            var curr = this;
            while (curr.HeadOption.GetIfPresent(out A head) && p(head) != isTruthly)
            {
                curr = curr.Tail;
            }

            return curr
                .HeadOption
                .Map(head => Stream(head, () => curr.Filter(p, isTruthly)))
                .GetOrElse(EmptyStream<A>());
        }
        private Stream<A> TakeWhile(Func<A, bool> p, bool isTruthly)
        {
            if (!HeadOption.GetIfPresent(out A head) || p(head) != isTruthly)
            {
                return EmptyStream<A>();
            }

            return Stream(head, () => Tail.TakeWhile(p, isTruthly));
        }

        IStream<A> IStream<A>.Tail => Tail;

        IStream<A> IStream<A>.Append(A item) => Append(item);
        IStream<A> IStream<A>.Append(ITraversable<A> traversable) => Append(traversable);
        IGrowable<A> IGrowable<A>.Append(A item) => Append(item);
        IGrowable<A> IGrowable<A>.Append(ITraversable<A> traversable) => Append(traversable);
        IStream<B> IStream<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f);
        IStream<B> IStream<A>.Map<B>(Func<A, B> f) => Map(f);
        IStream<A> IStream<A>.Prepend(A item) => Prepend(item);
        IStream<IStream<A>> IStream<A>.Sliding(int size, int step) => Sliding(size, step).As<IStream<A>>();
        IStream<A> IStream<A>.Prepend(ITraversable<A> traversable) => Prepend(traversable);
        IGrowable<A> IGrowable<A>.Prepend(A item) => Prepend(item);
        IGrowable<A> IGrowable<A>.Prepend(ITraversable<A> traversable) => Prepend(traversable);
        IStream<(A, B)> IStream<A>.Zip<B>(ICollection<B> other) => Zip(other);
        IStream<(A, int)> IStream<A>.ZipWithIndex() => ZipWithIndex();

        // inner classes

        internal sealed class EmptyStream : Stream<A>
        {
            public override Option<A> HeadOption => None<A>();
            public override Stream<A> Tail => this;
        }

        internal sealed class NonEmptyStream : Stream<A>
        {
            private readonly Lazy<Stream<A>> lazyTail;

            public NonEmptyStream(A head, Func<Stream<A>> tailFunc)
            {
                Head = head;
                lazyTail = tailFunc;
            }
            public NonEmptyStream(A head, Stream<A> tail) : this(head, () => tail) { }

            public A Head { get; }

            public override Option<A> HeadOption => Some(Head);
            public override Stream<A> Tail => lazyTail;
        }
    }
}
