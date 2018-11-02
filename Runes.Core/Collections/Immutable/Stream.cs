using System;
using System.Collections.Generic;
using System.Numerics;

namespace Runes.Collections.Immutable
{
    public static class Stream
    {
        public static IStream<A> Empty<A>() => Stream<A>.Empty;

        public static IStream<A> From<A>(A head, Lazy<IStream<A>> tail) => new ConsStream<A>(head, tail);

        public static IStream<A> From<A>(A head) => new ConsStream<A>(head, Empty<A>());

        public static IStream<A> From<A>(IEnumerable<A> e)
        {
            IStream<A> fromIterator(IEnumerator<A> iter) =>
                iter.MoveNext()
                    ? From(iter.Current, Lazy.From(() => fromIterator(iter)))
                    : Empty<A>();

            return fromIterator(e.GetEnumerator());
        }

        public static IStream<A> Flatten<A, That>(this IStream<That> stream) where That: Traversable<A> => 
            stream
                .HeadOption
                .Map(trav => trav.ToStream().Append(stream.Tail.Flatten<A, That>()))
                .GetOrElse(Empty<A>());
    }

    public interface IStream<A> : ITraversable<A>
    {
        bool IsEmpty { get; }
        bool NonEmpty { get; }
        Option<A> HeadOption { get; }
        IStream<A> Tail { get; }

        IStream<A> Append(A rear);
        IStream<A> Append(IEnumerable<A> e);
        IStream<B> Collect<B>(IPartialFunction<A, B> pf);
        Option<B> CollectFirst<B>(IPartialFunction<A, B> pf);
        bool Correspond<B>(IStream<B> another, Func<A, B, bool> p);
        IStream<A> Drops(int count);
        IStream<A> DropsWhile(Func<A, bool> p);
        IStream<A> DropsWhileNot(Func<A, bool> p);
        IStream<A> DropsWhile(Func<A, bool> p, out int skipped);
        IStream<A> DropsWhileNot(Func<A, bool> p, out int skipped);
        bool Exists(Func<A, bool> p);
        IStream<A> Filter(Func<A, bool> p);
        IStream<A> FilterNot(Func<A, bool> p);
        IStream<B> FlatMap<B, That>(Func<A, That> f) where That : Traversable<B>;
        bool ForAll(Func<A, bool> p);
        IStream<B> Map<B>(Func<A, B> f);
        IStream<A> Prepend(A e);
        IStream<A> Prepend(IEnumerable<A> e);
        BigInteger Size();
        IStream<A> Take(int count);
        IStream<A> TakeWhile(Func<A, bool> p);
        IStream<A> TakeWhileNot(Func<A, bool> p);
        IStream<(A, B)> Zip<B>(IStream<B> another);
        IStream<(A, int)> ZipWithIndex();
    }

    public abstract class Stream<A> : Traversable<A>, IStream<A>
    {
        internal static readonly EmptyStream<A> Empty = new EmptyStream<A>();

        public bool IsEmpty => HeadOption.IsEmpty;

        public bool NonEmpty  => HeadOption.NonEmpty;

        public abstract Option<A> HeadOption { get; }

        public abstract IStream<A> Tail { get; }

        public IStream<A> Append(A rear) =>
            HeadOption
                .Map(head => Stream.From(head, Lazy.From(() => Tail.Append(rear))))
                .GetOrElse(Stream.From(rear));

        public IStream<A> Append(IEnumerable<A> e) =>
            HeadOption
                .Map(head => Stream.From(head, Lazy.From(() => Tail.Append(e))))
                .GetOrElse(Stream.From(e));

        public IStream<B> Collect<B>(IPartialFunction<A, B> pf) =>
            FlatMap<B, Option<B>>(it => Code.Try(() => pf.Apply(it)).ToOption());

        public Option<B> CollectFirst<B>(IPartialFunction<A, B> pf) => Collect(pf).HeadOption;

        public bool Correspond<B>(IStream<B> other, Func<A, B, bool> p)
        {
            IStream<A> streamA = this;
            IStream<B> streamB = other;
            while (streamA.HeadOption.GetIfPresent(out A headA) && streamB.HeadOption.GetIfPresent(out B headB) && p(headA, headB))
            {
                streamA = streamA.Tail;
                streamB = streamB.Tail;
            }

            return streamA.IsEmpty && streamB.IsEmpty;
        }

        public IStream<A> Drops(int count) =>
            count > 0
                ? HeadOption
                    .Map(head => Stream.From(head, Lazy.From(() => Tail.Drops(count -1))))
                    .GetOrElse(Stream.Empty<A>())
                : Stream.Empty<A>();

        public IStream<A> DropsWhile(Func<A, bool> p) => DropsWhile(p, out _);

        public IStream<A> DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p, out _);

        public IStream<A> DropsWhile(Func<A, bool> p, out int skipped) => DropsWhile(p, true, out skipped);

        public IStream<A> DropsWhileNot(Func<A, bool> p, out int skipped) => DropsWhile(p, false, out skipped);

        public bool Exists(Func<A, bool> p) => Filter(p).NonEmpty;

        public IStream<A> Filter(Func<A, bool> p) => Filter(this, p, true);

        public IStream<A> FilterNot(Func<A, bool> p) => Filter(this, p, false);

        public IStream<B> FlatMap<B, That>(Func<A, That> f) where That : Traversable<B> =>
            HeadOption
                .Map(head => f(head).ToStream().Append(Tail.FlatMap<B, That>(f)))
                .GetOrElse(Stream.Empty<B>());

        public bool ForAll(Func<A, bool> p)
        {
            IStream<A> stream = this;
            while (stream.HeadOption.GetIfPresent(out A head) && p(head))
            {
                stream = stream.Tail;
            }
            return stream.IsEmpty;
        }

        public override Unit Foreach(Action<A> action) => Unit.Of(() =>
        {
            IStream<A> curr = this;
            while (curr.HeadOption.GetIfPresent(out A head))
            {
                action(head);
                curr = curr.Tail;
            }
        });

        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit.Of(() =>
        {
            IStream<A> curr = this;
            while (curr.HeadOption.GetIfPresent(out A head) && p(head))
            {
                action(head);
                curr = curr.Tail;
            }
        });

        public override IEnumerator<A> GetEnumerator()
        {
            IStream<A> stream = this;
            while(stream.HeadOption.GetIfPresent(out A curr))
            {
                yield return curr;
                stream = stream.Tail;
            }
        }

        public IStream<B> Map<B>(Func<A, B> f) =>
            HeadOption
                .Map(head => Stream.From(f(head), Lazy.From(() => Tail.Map(f))))
                .GetOrElse(Stream.Empty<B>());

        public IStream<A> Prepend(A e) => Stream.From(e, Lazy.From<IStream<A>>(() => this));

        public IStream<A> Prepend(IEnumerable<A> e) => Stream.From(e).Append(this);

        public BigInteger Size() => FoldLeft(BigInteger.Zero, (sum, _) => sum + 1);

        public IStream<A> Take(int count) =>
            count > 0
                ? HeadOption
                    .Map(head => Stream.From(head, Lazy.From(() => Tail.Take(count - 1))))
                    .GetOrElse(Stream.Empty<A>())
                : Stream.Empty<A>();

        public IStream<A> TakeWhile(Func<A, bool> p) => TakeWhile(this, p, true);

        public IStream<A> TakeWhileNot(Func<A, bool> p) => TakeWhile(this, p, false);

        public override IStream<A> ToStream() => this;

        public IStream<(A, B)> Zip<B>(IStream<B> other) =>
            HeadOption.GetIfPresent(out A headA) && other.HeadOption.GetIfPresent(out B headB)
                ? Stream.From((headA, headB), Lazy.From(() => Tail.Zip(other.Tail)))
                : Stream.Empty<(A, B)>();

        public IStream<(A, int)> ZipWithIndex() =>
            HeadOption.GetIfPresent(out A head)
                ? Stream.From((head, 0), Lazy.From(() => Tail.ZipWithIndex()))
                : Stream.Empty<(A, int)>();

        private protected Stream() { }

        private IStream<A> Filter(IStream<A> stream, Func<A, bool> p, bool isTruthly)
        {
            IStream<A> currStream = stream;
            while (currStream.HeadOption.GetIfPresent(out A head) && p(head) != isTruthly)
            {
                currStream = currStream.Tail;
            }

            return currStream
                .HeadOption
                .Map(head => Stream.From(head, Lazy.From(() => Filter(currStream, p, isTruthly))))
                .GetOrElse(Stream.Empty<A>());
        }

        private IStream<A> DropsWhile(Func<A, bool> p, bool isTruthly, out int skipped)
        {
            IStream<A> curr = this;
            skipped = 0;
            while(curr.HeadOption.Exists(p) == isTruthly)
            {
                curr = curr.Tail;
                skipped++;
            }
            return curr;
        }

        private IStream<A> TakeWhile(IStream<A> stream, Func<A, bool> p, bool isTruthly) =>
            stream.HeadOption.GetIfPresent(out A head) && p(head) == isTruthly
                ? Stream.From(head, Lazy.From(() => TakeWhile(stream.Tail, p, isTruthly)))
                : Stream.Empty<A>();
    }

    internal sealed class EmptyStream<A> : Stream<A>
    {
        public override Option<A> HeadOption => Option.None<A>();
        public override IStream<A> Tail => this;

        internal EmptyStream() { }
    }

    internal sealed class ConsStream<A> : Stream<A>
    {
        public A Head { get; }

        public override Option<A> HeadOption => Option.Some(Head);
        public override IStream<A> Tail => lazyTail.Get();

        internal ConsStream(A head, Lazy<IStream<A>> tail)
        {
            Head = head;
            lazyTail = tail;
        }
        internal ConsStream(A head, Func<IStream<A>> tail)
        {
            Head = head;
            lazyTail = tail;
        }
        internal ConsStream(A head, IStream<A> tail)
        {
            Head = head;
            lazyTail = Lazy.From(() => tail);
        }

        private readonly Lazy<IStream<A>> lazyTail;
    }
}
