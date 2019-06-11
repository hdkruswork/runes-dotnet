using System;
using System.Collections.Generic;
using System.Text;

using static Runes.Predef;

namespace Runes.Collections
{
    public abstract class ArrayBase<A, CC> : TraversableBase<A, CC>, IArray<A, CC> where CC : ArrayBase<A, CC>
    {
        public static CC CreateArrayFrom(ITraversable<A> traversable, Func<A[], CC> buildFunc)
        {
            var arr = new A[traversable.Size];
            traversable.FoldLeft(0, (idx, it) =>
            {
                arr[idx] = it;
                return idx + 1;
            });

            return buildFunc(arr);
        }

        public virtual Option<A> this[long idx]
        {
            get
            {
                var physicalIdx = GetPhysicalIdx(idx);
                return lowestIdx <= physicalIdx && physicalIdx <= highestIdx
                    ? Some(array[physicalIdx])
                    : None<A>();
            }
        }

        public override Option<A> HeadOption => this[0];
        public override CC Tail
        {
            get
            {
                (_, var tail) = Split(0);

                return tail;
            }
        }
        public CC Init
        {
            get
            {
                (var init, _) = Split(Length - 1);

                return init;
            }
        }
        public Option<A> Rear => this[Length - 1];

        public override long Size => Length;
        public long Length { get; }

        public Array<B> As<B>() where B : class
        {
            var list = FoldRight(EmptyList<B>(), (list, it) =>
            {
                var res = it is B b ? b : default;

                return list.Prepend(res);
            });

            return list.ToArray();
        }
        public Array<B> Collect<B>(Func<A, Option<B>> f)
        {
            var list = FoldRight(EmptyList<B>(), (list, it) =>
            {
                if (f(it).GetIfPresent(out B res))
                {
                    return list.Prepend(res);
                }

                return list;
            });

            return list.ToArray();
        }
        public override CC Filter(Func<A, bool> p) => Filter(p, true);
        public override CC FilterNot(Func<A, bool> p) => Filter(p, false);
        public override That FoldLeft<That>(That initialValue, Func<That, A, That> f)
        {
            var res = initialValue;
            for (long idx = 0; idx < Length; idx++)
            {
                var it = GetItemAt(idx);
                res = f(res, it);
            }

            return res;
        }
        public override That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var res = initialValue;
            for (long idx = 0; idx < Length; idx++)
            {
                var it = GetItemAt(idx);
                if (p(res, it))
                {
                    res = f(res, it);
                }
                else
                {
                    break;
                }
            }

            return res;
        }
        public That FoldRight<That>(That initialValue, Func<That, A, That> f)
        {
            var res = initialValue;
            for (long idx = Length - 1; idx >= 0; idx--)
            {
                var it = GetItemAt(idx);
                res = f(res, it);
            }

            return res;
        }
        public That FoldRightWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var res = initialValue;
            for (long idx = Length - 1; idx >= 0; idx--)
            {
                var it = GetItemAt(idx);
                if (p(res, it))
                {
                    res = f(res, it);
                }
                else
                {
                    break;
                }
            }

            return res;
        }
        public Unit For(Action<A, long> action) => Unit(() =>
        {
            for (long idx = 0; idx < Length; idx++)
            {
                var it = GetItemAt(idx);
                action(it, idx);
            }
        });
        public override CC Reversed() => CreateArray(array, GetPhysicalIdx(Length - 1), Length, -step);
        public ITraversable<CC> Sliding(int size, int step = 1)
        {
            var slides = EmptyList<CC>();
            for (long idx = 0; idx < Length; idx += (size + step - 1))
            {
                var startIdx = GetPhysicalIdx(idx);
                var adjustedSize = Math.Min(size, Length - idx);
                var slide = CreateArray(array, startIdx, adjustedSize, this.step);

                slides = slides.Prepend(slide);
            }

            return slides.ToArray().Reversed();
        }
        public (CC, CC) Split(long idx)
        {
            if (idx <= 0)
            {
                return (GetEmptyArray(), This);
            }
            else if (idx >= Length)
            {
                return (This, GetEmptyArray());
            }
            else
            {
                var firstStart = GetPhysicalIdx(0);
                var secondStart = GetPhysicalIdx(idx);
                return (CreateArray(array, firstStart, idx, step), CreateArray(array, secondStart, Length - idx, step));
            }
        }
        public override CC Take(int count)
        {
            if (count >= Length)
            {
                return This;
            }

            var list = EmptyList<A>();
            for (int idx = count; idx >= 0; idx--)
            {
                var it = GetItemAt(idx);
                list = list.Prepend(it);
            }

            return FromList(list);
        }
        public override CC TakeWhile(Func<A, bool> p) => TakeWhile(p, true);
        public override CC TakeWhileNot(Func<A, bool> p) => TakeWhile(p, false);

        // internal members

        protected internal ArrayBase(A[] array, long startIndex, long length, int step = 1)
        {
            this.array = array;
            this.startIndex = startIndex;
            Length = length;
            this.step = step;

            lowestIdx = Lazy(GetLowestPhysicalIdx);
            highestIdx = Lazy(GetHighestPhysicalIdx);
        }

        // protected members

        protected readonly A[] array;
        protected readonly long startIndex;
        protected readonly int step;

        protected readonly Lazy<long> lowestIdx;
        protected readonly Lazy<long> highestIdx;

        protected A GetItemAt(long logicalIdx) => array[GetPhysicalIdx(logicalIdx)];
        protected long GetPhysicalIdx(long logicalIdx) => startIndex + step * logicalIdx;

        protected abstract CC GetEmptyArray();
        protected abstract CC CreateArray(A[] array, long startIndex, long length, int step);
        protected abstract CC FromList(List<A> list);

        protected That FlatMap<B, That>(Func<A, ICollection<B>> f, Func<B[], That> buildFunc) where That : ArrayBase<B, That>
        {
            var list = FoldLeft(EmptyList<B>(), (list, it) =>
            {
                f(it).ToTraversable().Foreach(b => list = list.Prepend(b));

                return list;
            });

            return ArrayBase<B, That>.CreateArrayFrom(list, buildFunc).Reversed();
        }
        protected That Map<B, That>(Func<A, B> f, Func<B[], That> buildFunc) where That : ArrayBase<B, That>
        {
            var list = FoldRight(EmptyList<B>(), (l, it) => l.Prepend(f(it)));

            return ArrayBase<B, That>.CreateArrayFrom(list, buildFunc);
        }
        protected (Left, Right) Unzip<X, Y, Left, Right>(
            Func<A, (X, Y)> toPairFunc,
            Func<X[], Left> leftBuildFunc,
            Func<Y[], Right> rightBuildFunc
        ) where Left : ArrayBase<X, Left> where Right : ArrayBase<Y, Right>
        {
            (var left, var right) = FoldRight((EmptyList<X>(), EmptyList<Y>()), (listPair, it) =>
            {
                (var left, var right) = listPair;
                (var x, var y) = toPairFunc(it);

                return (left.Prepend(x), right.Prepend(y));
            });

            return
            (
                ArrayBase<X, Left>.CreateArrayFrom(left, leftBuildFunc),
                ArrayBase<Y, Right>.CreateArrayFrom(right, rightBuildFunc)
            );
        }
        protected Zipped Zip<B, Zipped>(ICollection<B> other, Func<(A, B)[], Zipped> buildFunc)
            where Zipped : ArrayBase<(A, B), Zipped>
        {
            var list = EmptyList<(A, B)>();
            var currThis = this;
            var currOther = other;
            while (currThis.HeadOption.GetIfPresent(out A a) && currOther.HeadOption.GetIfPresent(out B b))
            {
                list = list.Prepend((a, b));
                currThis = currThis.Tail;
                currOther = currOther.Tail;
            }

            return ArrayBase<(A, B), Zipped>.CreateArrayFrom(list, buildFunc).Reversed();
        }
        protected Zipped ZipWithIndex<Zipped>(Func<(A, int)[], Zipped> buildFunc)
            where Zipped : ArrayBase<(A, int), Zipped>
        {
            var list = EmptyList<(A, int)>();
            for (int idx = (int)Length; idx >= 0; idx++)
            {
                var it = GetItemAt(idx);
                list = list.Prepend((it, idx));
            }

            return ArrayBase<(A, int), Zipped>.CreateArrayFrom(list, buildFunc);
        }

        protected abstract IArray<B> ArrayFlatMap<B>(Func<A, ICollection<B>> f);
        protected abstract IArray<B> ArrayMap<B>(Func<A, B> f);
        protected abstract (IArray<X>, IArray<Y>) ArrayUnzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        protected abstract IArray<(A, B)> ArrayZip<B>(ICollection<B> other);
        protected abstract IArray<(A, int)> ArrayZipWithIndex();

        protected override ITraversable<B> TraversableAs<B>() where B : class => As<B>();
        protected override ITraversable<B> TraversableCollect<B>(Func<A, Option<B>> f) => Collect(f);
        protected override ITraversable<B> TraversableFlatMap<B>(Func<A, ICollection<B>> f) => ArrayFlatMap(f);
        protected override ITraversable<B> TraversableMap<B>(Func<A, B> f) => ArrayMap(f);
        protected override (ITraversable<X>, ITraversable<Y>) TraversableUnzip<X, Y>(Func<A, (X, Y)> toPairFunc) => ArrayUnzip(toPairFunc);
        protected override ITraversable<(A, B)> TraversableZip<B>(ICollection<B> other) => ArrayZip(other);
        protected override ITraversable<(A, int)> TraversableZipWithIndex() => ArrayZipWithIndex();

        // private members

        private long GetLowestPhysicalIdx() => step >= 0 ? startIndex : startIndex - Length + 1;
        private long GetHighestPhysicalIdx() => step >= 0 ? startIndex + Length - 1 : startIndex;

        private CC Filter(Func<A, bool> p, bool isTruthly)
        {
            bool filtered = false;
            var list = FoldRight(EmptyList<A>(), (list, it) =>
            {
                if (p(it) == isTruthly)
                {
                    return list.Prepend(it);
                }

                filtered = true;
                return list;
            });

            return filtered ? FromList(list) : This;
        }
        private CC TakeWhile(Func<A, bool> p, bool isTruthly)
        {
            var list = FoldLeftWhile(
                EmptyList<A>(),
                (_, it) => p(it) == isTruthly,
                (l, it) => l.Prepend(it)
            );

            return FromList(list).Reversed();
        }

        IArray<A> IArray<A>.Init => Init;
        CC IArray<A, CC>.Init => Init;
        Option<A> IArray<A>.Rear => Rear;
        long IArray<A>.Length => Length;
        Option<A> IArray<A>.this[long index] => this[index];

        IArray<B> IArray<A>.As<B>() => As<B>();
        IArray<B> IArray<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f);
        IArray<B> IArray<A>.FlatMap<B>(Func<A, ICollection<B>> f) => ArrayFlatMap(f);
        IArray<B> IArray<A>.Map<B>(Func<A, B> f) => ArrayMap(f);
        ITraversable<IArray<A>> IArray<A>.Sliding(int size, int step) => Sliding(size, step).As<IArray<A>>();
        (IArray<A>, IArray<A>) IArray<A>.Split(long index) => Split(index);
        (IArray<X>, IArray<Y>) IArray<A>.Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) => ArrayUnzip(toPairFunc);
        IArray<(A, B)> IArray<A>.Zip<B>(ICollection<B> other) => ArrayZip(other);
        IArray<(A, int)> IArray<A>.ZipWithIndex() => ArrayZipWithIndex();

        (CC, CC) IArray<A, CC>.Split(long index) => Split(index);
        ITraversable<CC> IArray<A, CC>.Sliding(int size, int step) => Sliding(size, step);
        That IArray<A>.FoldRight<That>(That initialValue, Func<That, A, That> f) => FoldRight(initialValue, f);
        That IArray<A>.FoldRightWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f) => FoldRightWhile(initialValue, p, f);
    }
}
