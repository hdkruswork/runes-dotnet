using System;
using System.Collections;
using System.Collections.Generic;
using static Runes.Predef;

namespace Runes.Collections
{
    public abstract class TraversableBase<A, CC> : CollectionBase<A, CC>, ITraversable<A, CC>
        where CC : TraversableBase<A, CC>
    {
        public virtual long Size => NonEmpty ? Tail.Size + 1 : 0;

        public override CC Drops(int count)
        {
            (var res, _) = FoldLeftWhile(
                (This, count),
                (pair, it) =>
                {
                    (_, var skipped) = pair;

                    return skipped < count;
                },
                (pair, _) =>
                {
                    (var list, var skipped) = pair;

                    return (list.Tail, skipped + 1);
                }
            );

            return res;
        }

        public override CC DropsWhile(Func<A, bool> p) => DropsWhile(p, out int _, true);

        public override CC DropsWhile(Func<A, bool> p, out int skipped) => DropsWhile(p, out skipped, true);

        public override CC DropsWhileNot(Func<A, bool> p) => DropsWhile(p, out int _, false);

        public override CC DropsWhileNot(Func<A, bool> p, out int skipped) => DropsWhile(p, out skipped, false);

        public override bool Exist(Func<A, bool> p) =>
            FoldLeftWhile(false, (res, _) => !res, (_, it) => p(it));

        public virtual bool ForAll(Func<A, bool> p) =>
            FoldLeftWhile(true, (res, _) => res, (_, it) => p(it));

        public virtual That FoldLeft<That>(That initialValue, Func<That, A, That> f) =>
            FoldLeftWhile(initialValue, (agg, it) => true, f);

        public virtual That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var res = initialValue;
            var curr = this;
            while (curr.HeadOption.GetIfPresent(out A head) && p(res, head))
            {
                res = f(res, head);
                curr = curr.Tail;
            }

            return res;
        }

        public virtual Unit Foreach(Action<A> action) => FoldLeft(Unit(), (_, it) =>
            UnitFunc(action)(it));

        public virtual Unit ForeachWhile(Func<A, bool> p, Action<A> action) =>
            FoldLeftWhile(Unit(), (_, it) => p(it), (_, it) => UnitFunc(action)(it));

        public IEnumerator<A> GetEnumerator()
        {
            var curr = this;
            while (curr.HeadOption.GetIfPresent(out A a))
            {
                yield return a;
                curr = curr.Tail;
            }
        }

        public abstract CC Reversed();

        // protected members

        protected CC DropsWhile(Func<A, bool> p, out int skipped, bool isTruthly)
        {
            (var res, var skp) = FoldLeftWhile(
                (This, 0),
                (_, it) => p(it) == isTruthly,
                (pair, _) =>
                {
                    (var cc, var skp) = pair;

                    return (cc.Tail, skp + 1);
                }
            );

            skipped = skp;
            return res;
        }
        protected CC Filter<CB>(Func<A, bool> p, bool isTruthly, CB builder) where CB : ICollectionBuilder<A, CC>
        {
            FoldLeftWhile(
                Unit(),
                (_, it) => p(it) == isTruthly,
                (_, it) => Unit(() => builder.Add(it))
            );

            return builder.Build();
        }
        protected CC FlatMap<CB>(Func<A, ICollection<A>> f, CB builder) where CB : ICollectionBuilder<A, CC>
        {
            Foreach(it =>
            {
                var collection = f(it);
                while (collection.HeadOption.GetIfPresent(out A item))
                {
                    builder.Add(item);
                    collection = collection.Tail;
                }
            });

            return builder.Build();
        }
        protected CC Take<CB>(int count, CB builder) where CB : ICollectionBuilder<A, CC>
        {
            FoldLeftWhile(
                count,
                (remaining, _) => remaining > 0,
                (remaining, it) =>
                {
                    builder.Add(it);
                    return remaining - 1;
                }
            );

            return builder.Build();
        }
        protected CC TakeWhile<CB>(Func<A, bool> p, bool isTruthly, CB builder) where CB : ICollectionBuilder<A, CC>
        {
            FoldLeftWhile(
                Unit(),
                (_, it) => p(it) == isTruthly,
                (_, it) => Unit(() => builder.Add(it))
            );

            return builder.Build();
        }

        protected That As<B, That, CB>(CB builder)
            where B : class
            where That : CollectionBase<B, That>
            where CB : ICollectionBuilder<B, That>
        {
            Foreach(a =>
            {
                a.As<B>().Foreach(b => builder.Add(b));
            });

            return builder.Build();
        }
        protected That Collect<B, That, CB>(Func<A, Option<B>> f, CB builder)
            where That : CollectionBase<B, That>
            where CB : ICollectionBuilder<B, That>
        {
            Foreach(a =>
            {
                f(a).Foreach(b => builder.Add(b));
            });

            return builder.Build();
        }
        protected That FlatMap<B, That, CB>(Func<A, ICollection<B>> f, CB builder)
            where That : CollectionBase<B, That>
            where CB : ICollectionBuilder<B, That>
        {
            Foreach(a =>
            {
                var traversable = f(a).ToTraversable();
                traversable.Foreach(b => builder.Add(b));
            });

            return builder.Build();
        }
        protected That Map<B, That, CB>(Func<A, B> f, CB builder)
            where That : CollectionBase<B, That>
            where CB : ICollectionBuilder<B, That>
        {
            Foreach(a => builder.Add(f(a)));

            return builder.Build();
        }
        protected (Left, Right) Unzip<X, Y, Left, Right, CBLeft, CBRight>(
            Func<A, (X, Y)> toPairFunc,
            CBLeft leftBuilder,
            CBRight rightBuilder)
            where Left : CollectionBase<X, Left>
            where Right : CollectionBase<Y, Right>
            where CBLeft : ICollectionBuilder<X, Left>
            where CBRight : ICollectionBuilder<Y, Right>
        {
            Foreach(a =>
            {
                (var x, var y) = toPairFunc(a);

                leftBuilder.Add(x);
                rightBuilder.Add(y);
            });

            return (leftBuilder.Build(), rightBuilder.Build());
        }
        protected Zipped Zip<B, Zipped, CB>(ICollection<B> other, CB builder)
            where Zipped : CollectionBase<(A, B), Zipped>
            where CB : ICollectionBuilder<(A, B), Zipped>
        {
            var currA = this;
            var currB = other;
            while (currA.HeadOption.Zip(currB.HeadOption).GetIfPresent(out (A, B) pair))
            {
                builder.Add(pair);
                currA = currA.Tail;
                currB = currB.Tail;
            }

            return builder.Build();
        }
        protected Zipped ZipWithIndex<Zipped, CB>(CB builder)
            where Zipped : CollectionBase<(A, int), Zipped>
            where CB : ICollectionBuilder<(A, int), Zipped>
        {
            FoldLeft(0, (idx, a) =>
            {
                builder.Add((a, idx));
                return idx + 1;
            });

            return builder.Build();
        }

        protected abstract ITraversable<B> TraversableAs<B>() where B : class;
        protected abstract ITraversable<B> TraversableCollect<B>(Func<A, Option<B>> f);
        protected abstract ITraversable<B> TraversableFlatMap<B>(Func<A, ICollection<B>> f);
        protected abstract ITraversable<B> TraversableMap<B>(Func<A, B> f);
        protected abstract (ITraversable<X>, ITraversable<Y>) TraversableUnzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        protected abstract ITraversable<(A, B)> TraversableZip<B>(ICollection<B> other);
        protected abstract ITraversable<(A, int)> TraversableZipWithIndex();

        protected override ICollection<B> CollectionAs<B>() where B : class => TraversableAs<B>();
        protected override ICollection<B> CollectionCollect<B>(Func<A, Option<B>> f) =>
            TraversableCollect(f);
        protected override ICollection<B> CollectionFlatMap<B>(Func<A, ICollection<B>> f) => TraversableFlatMap(f);
        protected override ICollection<B> CollectionMap<B>(Func<A, B> f) =>
            TraversableMap(f);
        protected override (ICollection<X>, ICollection<Y>) CollectionUnzip<X, Y>(Func<A, (X, Y)> toPairFunc) =>
            TraversableUnzip(toPairFunc);
        protected override ICollection<(A, B)> CollectionZip<B>(ICollection<B> other) =>
            TraversableZip(other);
        protected override ICollection<(A, int)> CollectionZipWithIndex() =>
            TraversableZipWithIndex();

        // private members

        ITraversable<A> ITraversable<A>.Tail => Tail;

        ITraversable<B> ITraversable<A>.As<B>() => TraversableAs<B>();
        ITraversable<B> ITraversable<A>.Collect<B>(Func<A, Option<B>> f) => TraversableCollect(f);
        ITraversable<B> ITraversable<A>.FlatMap<B>(Func<A, ICollection<B>> f) => TraversableFlatMap(f);
        ITraversable<B> ITraversable<A>.Map<B>(Func<A, B> f) => TraversableMap(f);
        ITraversable<A> ITraversable<A>.Reversed() => Reversed();
        (ITraversable<X>, ITraversable<Y>) ITraversable<A>.Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) => TraversableUnzip(toPairFunc);
        ITraversable<(A, B)> ITraversable<A>.Zip<B>(ICollection<B> other) => TraversableZip(other);
        ITraversable<(A, int)> ITraversable<A>.ZipWithIndex() => TraversableZipWithIndex();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
