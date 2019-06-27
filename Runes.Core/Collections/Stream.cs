using System;
using System.Collections.Generic;
using Runes.Math;
using static Runes.Predef;

namespace Runes.Collections
{
    public abstract class Stream<A> : IterableBase<A, Stream<A>>, IStream<A, Stream<A>>
    {
        public static readonly IFactory<A, Stream<A>> Factory = new StreamFactory();

        public static readonly Stream<A> Empty = new EmptyStream();

        public static Stream<A> Create(A head) => new NonEmptyStream(head, () => Empty);

        public static Stream<A> Create(A head, Func<Stream<A>> tailFunc) => new NonEmptyStream(head, tailFunc);

        public Stream<B> As<B>() =>
            HeadOption.GetIfPresent(out var head)
                ? head.As<B>(out var b) ? Stream<B>.Create(b, () => Tail.As<B>()) : Tail.As<B>()
                : Stream<B>.Empty;

        public Stream<A> Append(A item) =>
            HeadOption.GetIfPresent(out var head)
                ? Create(head, () => Tail.Append(item))
                : Create(item);

        public Stream<A> Append(IIterable<A> iterable) =>
            HeadOption.GetIfPresent(out var head)
                ? Create(head, () => Tail.Append(iterable))
                : iterable.ToStream();

        public Stream<A> Append(Func<Stream<A>> streamFunc) =>
            HeadOption.GetIfPresent(out var head)
                ? Create(head, () => Tail.Append(streamFunc))
                : streamFunc();

        public Stream<B> Collect<B>(Func<A, Option<B>> f) =>
            HeadOption.GetIfPresent(out var head)
                ? f(head).GetIfPresent(out var b) ? Stream<B>.Create(b, () => Tail.Collect(f)) : Tail.Collect(f)
                : Stream<B>.Empty;

        public Stream<B> FlatMap<B>(Func<A, IIterable<B>> f) =>
            HeadOption.GetIfPresent(out var head)
                ? f(head).ToStream().Append(() => Tail.FlatMap(f))
                : Stream<B>.Empty;

        public Option<A> GetAt(Int idx)
        {
            var (res, _) =
                FoldLeftWhile(
                    (None<A>(), idx),
                    (pair, _) =>
                    {
                        var (_, i) = pair;
                        return i >= 0;
                    },
                    (pair, it) =>
                    {
                        var (_, i) = pair;
                        return (i == 0 ? (Some(it)) : None<A>(), i - 1);
                    }
                );

            return res;
        }

        public Stream<B> Map<B>(Func<A, B> f) =>
            HeadOption.GetIfPresent(out var head)
                ? Stream<B>.Create(f(head), () => Tail.Map(f))
                : Stream<B>.Empty;

        public override (Stream<A>, Stream<A>) Partition(Func<A, bool> p)
        {
            var (left, right) =
                Unzip(it => p(it) ? ((Option<A>)Some(it), None<A>()) : (None<A>(), Some(it)));

            return (left.Flatten(), right.Flatten());
        }

        public Stream<A> Prepend(A item) => Create(item, () => this);

        public Stream<A> Prepend(IIterable<A> iterable) =>
            iterable.ToStream().Append(this);

        public Stream<Array<A>> Slice(int size, int nextStep)
        {
            var currStep = nextStep > 0 ? nextStep : 1;
            var head = Take(size).ToArray();
            var tail = Drops(currStep);

            return nextStep > 0 && head.NonEmpty
                ? Stream(head, () => tail.Slice(size, currStep))
                : Stream<Array<A>>.Empty;
        }

        public override Stream<A> Take(Int count) =>
            count > 0 && HeadOption.GetIfPresent(out var head)
                ? Create(head, () => Tail.Take(count - 1))
                : Empty;

        public (Stream<X>, Stream<Y>) Unzip<X, Y>(Func<A, (X, Y)> f)
        {
            var zipped = Map(f);

            return (zipped.Map(pair => pair.Item1), zipped.Map(pair => pair.Item2));
        }

        public Stream<(A, B)> Zip<B>(IIterable<B> other) =>
            HeadOption.GetIfPresent(out var a) && other.HeadOption.GetIfPresent(out var b)
                ? Stream<(A, B)>.Create((a, b), () => Tail.Zip(other.Tail))
                : Stream<(A, B)>.Empty;

        public Stream<(A, Int)> ZipWithIndex() => Zip(Int.GetValuesFrom(0));

        // protected members

        protected override Stream<A> Filter(Func<A, bool> p, bool isAffirmative) =>
            HeadOption.GetIfPresent(out var head)
                ? p(head) == isAffirmative ? Create(head, () => Tail.Filter(p, isAffirmative)) : Tail.Filter(p, isAffirmative)
                : Empty;

        protected override IFactory<A, Stream<A>> GetFactory() => Factory;

        protected override IFactory<B, IIterable<B>> GetFactory<B>() => Stream<B>.Factory;

        protected override Stream<A> TakeWhile(Func<A, bool> p, bool isAffirmative) =>
            HeadOption.GetIfPresent(out var head) && p(head) == isAffirmative
                ? Create(head, () => Tail.TakeWhile(p, isAffirmative))
                : Empty;

        // private members

        private protected Stream() { }

        IStream<A> IStream<A>.Tail => Tail;

        IIndexable<A> IIndexable<A>.Tail => Tail;

        IStream<A> IStream<A>.Append(Func<IStream<A>> streamFunc) => Append(() => (Stream<A>)streamFunc());

        IStream<B> IStream<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f);

        (IStream<X>, IStream<Y>) IStream<A>.Unzip<X, Y>(Func<A, (X, Y)> f) => Unzip(f);

        IStream<(A, B)> IStream<A>.Zip<B>(IIterable<B> other) => Zip(other);

        IStream<(A, Int)> IStream<A>.ZipWithIndex() => ZipWithIndex();

        bool IGrowable<A>.Accepts(A item) => true;

        IStream<A> IStream<A>.Append(IIterable<A> iterable) => Append(iterable);

        IGrowable<A> IGrowable<A>.Append(A item) => Append(item);

        IGrowable<A> IGrowable<A>.Append(IIterable<A> iterable) => Append(iterable);

        IStream<A> IStream<A>.Append(A item) => Append(item);

        IStream<A> IStream<A>.Drops(Int count) => Drops(count);

        IStream<A> IStream<A>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);

        IStream<A> IStream<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);

        IStream<A> IStream<A>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);

        IStream<A> IStream<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);

        IStream<A> IStream<A>.Filter(Func<A, bool> p) => Filter(p);

        IGrowable<A> ICollection<A, IGrowable<A>>.Filter(Func<A, bool> p) => Filter(p);

        IGrowable<A> ICollection<A, IGrowable<A>>.FilterNot(Func<A, bool> p) => FilterNot(p);

        IStream<A> IStream<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        IGrowable<A> IGrowable<A>.Prepend(IIterable<A> iterable) => Prepend(iterable);

        IGrowable<A> IGrowable<A>.Prepend(A item) => Prepend(item);

        IStream<A> IStream<A>.Prepend(A item) => Prepend(item);

        IStream<A> IStream<A>.Prepend(IIterable<A> iterable) => Prepend(iterable);

        IStream<IArray<A>> IStream<A>.Slice(int size, int nextStep) => Slice(size, nextStep).As<IArray<A>>();

        IStream<A> IStream<A>.Take(Int count) => Take(count);

        IStream<A> IStream<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);

        IStream<A> IStream<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);

        IIndexable<(A, Int)> IIndexable<A>.ZipWithIndex() => ZipWithIndex();

        private sealed class NonEmptyStream : Stream<A>
        {
            public override Option<A> HeadOption { get; }

            public override Stream<A> Tail => LazyTail;

            public NonEmptyStream(A head, Func<Stream<A>> lazyTail)
            {
                HeadOption = Some(head);
                LazyTail = lazyTail;
            }

            private Lazy<Stream<A>> LazyTail { get; }
        }

        private sealed class EmptyStream : Stream<A>
        {
            public override Option<A> HeadOption => None<A>();

            public override Stream<A> Tail => this;
        }

        private sealed class StreamFactory : FactoryBase<A, Stream<A>>
        {
            public override IIterableBuilder<A, Stream<A>> NewBuilder() => new Builder();

            public override Stream<A> From(IEnumerable<A> e)
            {
                if (e is Stream<A> stream)
                {
                    return stream;
                }

                Stream<A> GetStreamFrom(IEnumerator<A> enumerator) =>
                    enumerator.MoveNext()
                        ? Create(enumerator.Current, () => GetStreamFrom(enumerator))
                        : Empty;

                return GetStreamFrom(e.GetEnumerator());
            }
        }

        private sealed class Builder : Builder<Builder>
        {
            private List<A> innerList = EmptyList<A>();

            public override Builder Append(A item)
            {
                innerList = innerList.Prepend(item);
                return this;
            }

            public override Stream<A> Build() =>
                innerList
                    .Reverse()
                    .ToStream();

            public override Stream<A> GetEmpty() => Empty;
        }
    }
}
