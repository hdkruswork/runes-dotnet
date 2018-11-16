using System;
using System.Collections.Generic;

namespace Runes.Collections.Immutable
{
    public interface ISeqViewLike<A, Repr> : ITraversable<A> where Repr: ISeqViewLike<A, Repr>
    {
        bool IsEmpty { get; }
        bool NonEmpty { get; }

        Option<A> HeadOption { get; }
        Repr Tail { get; }

        That Collect<B, That>(IPartialFunction<A, B> pf, ICollectionBuilder<B, That> bf) where That: ISeqViewLike<B, That>;
        Option<B> CollectFirst<B>(IPartialFunction<A, B> pf);
        bool Correspond<B, That>(That other, Func<A, B, bool> p) where That: ISeqViewLike<B, That>;
        Repr Drops(int count);
        Repr DropsWhile(Func<A, bool> p);
        Repr DropsWhileNot(Func<A, bool> p);
        Repr DropsWhile(Func<A, bool> p, out int skipped);
        Repr DropsWhileNot(Func<A, bool> p, out int skipped);
        bool Exists(Func<A, bool> p);
        Repr Filter(Func<A, bool> p);
        Repr FilterNot(Func<A, bool> p);
        R FlatMap<B, That, R>(Func<A, That> f, ICollectionBuilder<B, R> bf) where That : ITraversable<B> where R: ISeqViewLike<B, R>;
        bool ForAll(Func<A, bool> p);
        bool GetHeadIfPresent(out A head);
        That Map<B, That>(Func<A, B> f, ICollectionBuilder<B, That> bf) where That: ISeqViewLike<B, That>;
        Repr Reverse();
        int Size();
        Repr Take(int count);
        Repr TakeWhile(Func<A, bool> p);
        Repr TakeWhileNot(Func<A, bool> p);
        R Zip<B, That, R>(ISeqViewLike<B, That> another, ICollectionBuilder<(A, B), R> b)
            where That : ISeqViewLike<B, That> where R : ISeqLike<(A, B), R>;
        That ZipWithIndex<That>(ICollectionBuilder<(A, int), That> b) where That: ISeqViewLike<(A, int), That>;
    }

    public abstract class SeqViewLike<A, Repr>
        : Traversable<A>, ISeqViewLike<A, Repr> where Repr : SeqViewLike<A, Repr>
    {
        public virtual bool IsEmpty => HeadOption.IsEmpty;

        public bool NonEmpty => !IsEmpty;

        public abstract Option<A> HeadOption { get; }

        public abstract Repr Tail { get; }

        public virtual Option<B> CollectFirst<B>(IPartialFunction<A, B> pf)
        {
            foreach (var item in this)
            {
                if (Code.IsSuccess(() => pf.Apply(item), out B res))
                {
                    return Option.Some(res);
                }
            }

            return Option.None<B>();
        }

        public virtual bool Correspond<B, That>(That other, Func<A, B, bool> p) where That : ISeqViewLike<B, That>
        {
            Repr seqA = ThisAsRepr;
            That seqB = other;
            while (seqA.GetHeadIfPresent(out A headA) && seqB.GetHeadIfPresent(out B headB) && p(headA, headB))
            {
                seqA = seqA.Tail;
                seqB = seqB.Tail;
            }

            return seqA.IsEmpty && seqB.IsEmpty;
        }

        public virtual Repr Drops(int count)
        {
            var counter = count;
            var curr = ThisAsRepr;
            while (counter > 0 && curr.NonEmpty)
            {
                curr = curr.Tail;
                counter -= 1;
            }
            return curr;
        }

        public virtual Repr DropsWhile(Func<A, bool> p) => DropsWhile(p, true, out _);

        public virtual Repr DropsWhile(Func<A, bool> p, out int dropped) => DropsWhile(p, true, out dropped);

        public virtual Repr DropsWhileNot(Func<A, bool> p) => DropsWhile(p, false, out _);

        public virtual Repr DropsWhileNot(Func<A, bool> p, out int dropped) => DropsWhile(p, false, out dropped);

        public virtual bool Exists(Func<A, bool> p)
        {
            foreach (var item in this)
            {
                if (p(item)) return true;
            }
            return false;
        }

        public virtual Repr Filter(Func<A, bool> p) => Filter(p, true);

        public virtual Repr FilterNot(Func<A, bool> p) => Filter(p, false);

        public virtual bool ForAll(Func<A, bool> p)
        {
            foreach (var item in this)
            {
                if(!p(item)) return false;
            }
            return true;
        }

        public override Unit Foreach(Action<A> action) => Unit.Of(() =>
        {
            var curr = this;
            while (curr.GetHeadIfPresent(out A item))
            {
                action(item);
                curr = curr.Tail;
            }
        });

        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit.Of(() =>
        {
            var curr = this;
            while (curr.GetHeadIfPresent(out A item) && p(item))
            {
                action(item);
                curr = curr.Tail;
            }
        });

        public virtual bool GetHeadIfPresent(out A head) => HeadOption.GetIfPresent(out head);

        public override IEnumerator<A> GetEnumerator()
        {
            Repr curr = ThisAsRepr;
            while (curr.GetHeadIfPresent(out A head))
            {
                yield return head;
                curr = curr.Tail;
            }
        }

        public abstract Repr Reverse();

        public virtual int Size() => FoldLeft(0, (sum, _) => sum + 1);

        public virtual Repr Take(int count)
        {
            var builder = NewBuilder();
            var curr = ThisAsRepr;
            var counter = count;
            while (counter > 0 && curr.GetHeadIfPresent(out A elem))
            {
                builder = builder.Append(elem);
                counter -= 1;
            }
            return builder.Result();
        }

        public virtual Repr TakeWhile(Func<A, bool> p) => TakeWhile(p, true);

        public virtual Repr TakeWhileNot(Func<A, bool> p) => TakeWhile(p, false);

        public override Stream<A> ToStream() =>
            GetHeadIfPresent(out A head) ? Stream.Of(head, Lazy.From(() => Tail.ToStream())) : Stream.Empty<A>();

        // Protected members

        protected Repr ThisAsRepr => (Repr)this;

        protected abstract ICollectionBuilder<A, Repr> NewBuilder();

        protected virtual That Collect<B, That>(IPartialFunction<A, B> pf, ICollectionBuilder<B, That> builder)
            where That: ISeqViewLike<B, That>
        {
            var bf = builder;
            foreach (var item in this)
            {
                var tryRes = Code.Try(() => pf.Apply(item));
                if (tryRes.GetIfSuccess(out B res))
                {
                    bf.Append(res);
                }
            }
            return bf.Result();
        }

        protected virtual R FlatMap<B, That, R>(Func<A, That> f, ICollectionBuilder<B, R> bf)
            where That : ITraversable<B> where R: ISeqViewLike<B, R>
        {
            var builder = bf.NewBuilder();
            foreach (var item in this)
            {
                builder = builder.Append(f(item));
            }
            return builder.Result();
        }

        protected virtual That Map<B, That>(Func<A, B> f, ICollectionBuilder<B, That> bf)
            where That: ISeqViewLike<B, That>
        {
            var builder = bf.NewBuilder();
            foreach (var item in this)
            {
                builder = builder.Append(f(item));
            }
            return builder.Result();
        }
        
        protected virtual R Zip<B, That, R>(ISeqViewLike<B, That> another, ICollectionBuilder<(A, B), R> bf)
            where That : ISeqViewLike<B, That>
            where R : ISeqViewLike<(A, B), R>
        {
            var builder = bf.NewBuilder();
            var seqA = ThisAsRepr;
            var seqB = another;
            while (seqA.GetHeadIfPresent(out A elemA) && seqB.GetHeadIfPresent(out B elemB))
            {
                builder = builder.Append((elemA, elemB));
            }
            return builder.Result();
        }

        protected virtual That ZipWithIndex<That>(ICollectionBuilder<(A, int), That> bf) where That : ISeqViewLike<(A, int), That>
        {
            var idx = 0;
            var curr = ThisAsRepr;
            var builder = bf.NewBuilder();
            while (curr.GetHeadIfPresent(out A elem))
            {
                builder = builder.Append((elem, idx));
                idx += 1;
            }
            return builder.Result();
        }

        // Private members

        private Repr Filter(Func<A, bool> p, bool isTruthly)
        {
            var builder = NewBuilder();
            foreach (var item in this)
            {
                if (p(item) == isTruthly)
                {
                    builder.Append(item);
                }
            }
            return builder.Result();
        }

        private Repr DropsWhile(Func<A, bool> p, bool isTruthly, out int dropped)
        {
            dropped = 0;
            var curr = ThisAsRepr;
            while (curr.GetHeadIfPresent(out A elem) && p(elem) == isTruthly)
            {
                dropped += 1;
                curr = curr.Tail;
            }
            return curr;
        }

        private Repr TakeWhile(Func<A, bool> p, bool isTruthly)
        {
            var builder = NewBuilder();
            var curr = ThisAsRepr;
            while (curr.GetHeadIfPresent(out A elem) && p(elem) == isTruthly)
            {
                builder = builder.Append(elem);
                curr = curr.Tail;
            }
            return builder.Result();
        }

        That ISeqViewLike<A, Repr>.Collect<B, That>(IPartialFunction<A, B> pf, ICollectionBuilder<B, That> bf) => Collect(pf, bf);

        R ISeqViewLike<A, Repr>.FlatMap<B, That, R>(Func<A, That> f, ICollectionBuilder<B, R> bf) => FlatMap(f, bf);

        That ISeqViewLike<A, Repr>.Map<B, That>(Func<A, B> f, ICollectionBuilder<B, That> builder) => Map(f, builder);
        
        R ISeqViewLike<A, Repr>.Zip<B, That, R>(ISeqViewLike<B, That> another, ICollectionBuilder<(A, B), R> bf) => Zip(another, bf);

        That ISeqViewLike<A, Repr>.ZipWithIndex<That>(ICollectionBuilder<(A, int), That> bf) => ZipWithIndex(bf);
    }
}
