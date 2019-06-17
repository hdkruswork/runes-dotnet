using System;
using System.Collections;
using System.Collections.Generic;
using Runes.Math;

using static Runes.Predef;

namespace Runes.Collections
{
    public abstract class CollectionBase<A, THIS> : ICollection<A, THIS>
        where THIS : CollectionBase<A, THIS>
    {
        public abstract bool IsEmpty { get; }

        public bool NonEmpty => !IsEmpty;

        public abstract bool Contains(A item);

        public THIS Filter(Func<A, bool> p) => Filter(p, true);

        public THIS FilterNot(Func<A, bool> p) => Filter(p, false);

        // protected members

        protected THIS This => (THIS)this;

        protected abstract THIS Filter(Func<A, bool> p, bool isAffirmative);

        // private members

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);

        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
    }

    public abstract class IterableBase<A, THIS> : CollectionBase<A, THIS>, IIterable<A, THIS>
        where THIS : IterableBase<A, THIS>
    {
        public abstract Option<A> HeadOption { get; }

        public abstract THIS Tail { get; }

        public override bool IsEmpty => HeadOption.IsEmpty;

        public override bool Contains(A item) => Exists(it => it.Equals(item));

        public THIS Drops(Int count)
        {
            var current = This;
            while (NonEmpty && count > 0)
            {
                current = current.Tail;
                count -= 1;
            }

            return current;
        }

        public THIS DropsWhile(Func<A, bool> p) => DropsWhile(p, out _, true);

        public THIS DropsWhileNot(Func<A, bool> p) => DropsWhile(p, out _, false);

        public THIS DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped, true);

        public THIS DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped, false);

        public bool Exists(Func<A, bool> p) => Exists(p, true);

        public bool ExistsNot(Func<A, bool> p) => Exists(p, false);

        public virtual bool ForAll(Func<A, bool> p)
        {
            var current = This;
            while (current.HeadOption.GetIfPresent(out var head))
            {
                if (!p(head))
                {
                    return false;
                }

                current = current.Tail;
            }

            return true;
        }

        public virtual That FoldLeft<That>(That initialValue, Func<That, A, That> f) =>
            FoldLeftWhile(initialValue, (_, it) => true, f);

        public virtual That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var res = initialValue;
            var current = This;
            while (current.HeadOption.GetIfPresent(out var head) && p(res, head))
            {
                res = f(res, head);
                current = current.Tail;
            }

            return res;
        }

        public Unit Foreach(Action<A> action) => ForeachWhile(_ => true, action);

        public Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit(() =>
        {
            var current = This;
            while (current.HeadOption.GetIfPresent(out var head) && p(head))
            {
                action(head);
                current = current.Tail;
            }
        });

        public virtual IEnumerator<A> GetEnumerator()
        {
            var current = This;
            while (current.HeadOption.GetIfPresent(out var head))
            {
                yield return head;
                current = current.Tail;
            }
        }

        public virtual (THIS, THIS) Partition(Func<A, bool> p)
        {
            var factory = GetFactory();
            var leftBuilder = factory.NewBuilder();
            var rightBuilder = factory.NewBuilder();

            foreach (var item in this)
            {
                if (p(item))
                {
                    leftBuilder.Append(item);
                }
                else
                {
                    rightBuilder.Append(item);
                }
            }

            return (leftBuilder.Build(), rightBuilder.Build());
        }

        public virtual THIS Take(Int count)
        {
            var (colBuilder, _) = FoldLeftWhile(
                (GetFactory().NewBuilder(), count),
                (agg, _) =>
                {
                    var (_, ct) = agg;
                    return ct > 0;
                },
                (agg, it) =>
                {
                    var (builder, ct) = agg;
                    return (builder.Append(it), ct - 1);
                }
            );

            return colBuilder.Build();
        }

        public THIS TakeWhile(Func<A, bool> p) => TakeWhile(p, true);

        public THIS TakeWhileNot(Func<A, bool> p) => TakeWhile(p, false);

        public That To<That>(IFactory<A, That> factory) where That : IIterable<A> => factory.From(this);

        public Array<A> ToArray() => To(Array<A>.Factory);

        public List<A> ToList() => To(List<A>.Factory);

        public A[] ToMutableArray()
        {
            var list = ToList();
            var array = new A[(long)list.Size];
            list.ZipWithIndex().Foreach(pair =>
            {
                var (it, idx) = pair;
                array[(long)idx] = it;
            });

            return array;
        }

        public Stream<A> ToStream() => To(Stream<A>.Factory);

        public abstract class Builder<BB> : IIterableBuilder<A, THIS, BB> where BB : Builder<BB>
        {
            public abstract BB Append(A item);

            public abstract THIS Build();

            public abstract THIS GetEmpty();

            // private members

            IIterableBuilder<A, THIS> IIterableBuilder<A, THIS>.Append(A item) => Append(item);

            IIterableBuilder<A> IIterableBuilder<A>.Append(A item) => Append(item);

            IIterable<A> IIterableBuilder<A>.GetEmpty() => GetEmpty();

            IIterable<A> IBuilder<IIterable<A>>.Build() => Build();
        }

        // protected members

        protected That As<B, That>(IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = This;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                if (head.As<B>(out var b))
                {
                    builder.Append(b);
                }
                current = current.Tail;
            }

            return builder.Build();
        }

        protected That Collect<B, That>(Func<A, Option<B>> f, IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = This;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var option = f(head);
                option.Foreach(it => builder.Append(it));
                current = current.Tail;
            }

            return builder.Build();
        }

        protected virtual THIS DropsWhile(Func<A, bool> p, out Int dropped, bool isAffirmative)
        {
            var res = This;
            dropped = 0;
            while (res.HeadOption.GetIfPresent(out var head) && p(head) == isAffirmative)
            {
                res = res.Tail;
                dropped += 1;
            }

            return res;
        }

        protected virtual bool Exists(Func<A, bool> p, bool isAffirmative)
        {
            var current = This;
            while (current.HeadOption.GetIfPresent(out var head))
            {
                if (p(head) == isAffirmative)
                {
                    return true;
                }

                current = current.Tail;
            }
            return false;
        }

        protected override THIS Filter(Func<A, bool> p, bool isAffirmative) =>
            FoldLeft(GetFactory().NewBuilder(), (builder, item) => p(item) ? builder.Append(item) : builder)
                .Build();

        protected That FlatMap<B, That>(Func<A, IIterable<B>> f, IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = This;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var iterable = f(head);
                iterable.Foreach(it => builder.Append(it));
                current = current.Tail;
            }

            return builder.Build();
        }

        protected abstract IFactory<A, THIS> GetFactory();

        protected abstract IFactory<B, IIterable<B>> GetFactory<B>();

        protected That Map<B, That>(Func<A, B> f, IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = This;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var b = f(head);
                builder.Append(b);
                current = current.Tail;
            }

            return builder.Build();
        }

        protected virtual THIS TakeWhile(Func<A, bool> p, bool isAffirmative) =>
            FoldLeftWhile(GetFactory().NewBuilder(), (_, it) => p(it) == isAffirmative, (b, it) => b.Append(it))
                .Build();

        protected (Left, Right) Unzip<X, Y, Left, Right>(Func<A, (X, Y)> f, IFactory<X, Left> leftFactory, IFactory<Y, Right> rightFactory)
            where Left : IIterable<X>
            where Right : IIterable<Y>
        {
            var current = This;
            var xBuilder = leftFactory.NewBuilder();
            var yBuilder = rightFactory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var (x, y) = f(head);
                xBuilder.Append(x);
                yBuilder.Append(y);
                current = current.Tail;
            }

            return (xBuilder.Build(), yBuilder.Build());
        }

        protected That Zip<B, That>(IIterable<B> other, IFactory<(A, B), That> factory)
            where That : IIterable<(A, B)>
        {
            var currentThis = This;
            var currentOther = other;
            var builder = factory.NewBuilder();
            while (currentThis.HeadOption.GetIfPresent(out var a) && other.HeadOption.GetIfPresent(out var b))
            {
                builder.Append((a, b));
                currentThis = currentThis.Tail;
                currentOther = currentOther.Tail;
            }

            return builder.Build();
        }

        // Private members

        IIterable<A> IIterable<A>.Tail => Tail;

        IIterable<B> IIterable<A>.As<B>() => As(GetFactory<B>());

        IIterable<B> IIterable<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f, GetFactory<B>());

        IIterable<A> IIterable<A>.Drops(Int count) => Drops(count);

        IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);

        IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);

        IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);

        IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);

        IIterable<B> IIterable<A>.FlatMap<B>(Func<A, IIterable<B>> f) => FlatMap(f, GetFactory<B>());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IIterable<B> IIterable<A>.Map<B>(Func<A, B> f) => Map(f, GetFactory<B>());

        (IIterable<A>, IIterable<A>) IIterable<A>.Partition(Func<A, bool> p) => Partition(p);

        IIterable<A> IIterable<A>.Take(Int count) => Take(count);

        IIterable<A> IIterable<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);

        IIterable<A> IIterable<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);

        (IIterable<X>, IIterable<Y>) IIterable<A>.Unzip<X, Y>(Func<A, (X, Y)> f) =>
            Unzip(f, GetFactory<X>(), GetFactory<Y>());

        IIterable<(A, B)> IIterable<A>.Zip<B>(IIterable<B> other) => Zip(other, GetFactory<(A, B)>());
    }

    public abstract class EagerIterableBase<A, THIS> : IterableBase<A, THIS>, IEagerIterable<A, THIS>
        where THIS : EagerIterableBase<A, THIS>
    {
        public virtual Int Size => FoldLeft(Int.Zero, (count, _) => count + 1);

        public abstract THIS Reverse();

        public virtual THIS Sort(Ordering<A> ord) =>
            Quicksort<A>()
                .Sort(ToMutableArray(), ord)
                .To(GetFactory());

        IEagerIterable<A> IEagerIterable<A>.Reverse() => Reverse();

        IEagerIterable<A> IEagerIterable<A>.Sort(Ordering<A> ord) => Sort(ord);
    }

    public abstract class EagerGrowableBase<A, THIS> : EagerIterableBase<A, THIS>, IGrowable<A, THIS>
        where THIS : EagerGrowableBase<A, THIS>
    {
        public virtual bool Accepts(A item) => true;

        public virtual THIS Append(A item) => GrowableUtils.Append(item, This, GetFactory());

        public virtual THIS Append(IIterable<A> iterable) => GrowableUtils.Append(iterable, This, GetFactory());

        public abstract THIS Prepend(A item);

        public virtual THIS Prepend(IIterable<A> iterable) => GrowableUtils.Prepend(iterable, This, GetFactory());

        public override THIS Reverse() => 
            FoldLeft(GetFactory().NewBuilder().Build(), (agg, it) => agg.Prepend(it));

        // private members

        IGrowable<A> IGrowable<A>.Append(A item) => Append(item);

        IGrowable<A> IGrowable<A>.Append(IIterable<A> iterable) => Append(iterable);

        IGrowable<A> IGrowable<A>.Prepend(A item) => Prepend(item);

        IGrowable<A> IGrowable<A>.Prepend(IIterable<A> iterable) => Prepend(iterable);

        IGrowable<A> ICollection<A, IGrowable<A>>.Filter(Func<A, bool> p) => Filter(p);

        IGrowable<A> ICollection<A, IGrowable<A>>.FilterNot(Func<A, bool> p) => FilterNot(p);
    }

    public abstract class ArrayBase<A, THIS> : EagerIterableBase<A, THIS>, IArray<A, THIS>
        where THIS : ArrayBase<A, THIS>
    {
        public virtual Option<A> this[long idx]
        {
            get
            {
                var physicalIdx = GetPhysicalIdx(idx);
                return GetLowestPhysicalIdx() <= physicalIdx && physicalIdx <= GetHighestPhysicalIdx()
                    ? Some(array[physicalIdx])
                    : None<A>();
            }
        }

        public virtual Option<A> this[Index idx] => this[GetLogicalIdx(idx)];

        public override Option<A> HeadOption => this[0L];

        public Option<A> Rear => this[Length - 1];

        public THIS Init => SplitAt(Length - 1).Item1;

        public override THIS Tail => SplitAt(1).Item2;

        public long Length { get; }

        public override Int Size => Length;

        public Option<A> GetAt(Int idx) =>
            Try(() => this[(long) idx])
                .ToOption()
                .Flatten();

        public override That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var res = initialValue;
            
            A item;
            for (long idx = 0; idx < Length && p(res, item = GetItemAt(idx)); idx++)
            {
                res = f(res, item);
            }

            return res;
        }

        public That FoldRight<That>(That initialValue, Func<That, A, That> f) =>
            FoldRightWhile(initialValue, (_, it) => true, f);

        public That FoldRightWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var res = initialValue;

            A item;
            for (var idx = Length - 1; idx >= 0 && p(res, item = GetItemAt(idx)); idx--)
            {
                res = f(res, item);
            }

            return res;
        }

        public override THIS Reverse() =>
            GetArrayFactory()
                .From(array, GetPhysicalIdx(Length - 1), Length, -step);

        public IIterable<THIS> Slide(long size) => Slide(size, size);

        public IIterable<THIS> Slide(long size, long nextStep)
        {
            var listBuilder = List<THIS>.Factory.NewBuilder();
            var factory = GetArrayFactory();

            for (long idx = 0; idx < Length; idx += (size + nextStep - 1))
            {
                var startIdx = GetPhysicalIdx(idx);
                var adjustedSize = System.Math.Min(size, Length - idx);
                var slide = factory.From(array, startIdx, adjustedSize, step);

                listBuilder = listBuilder.Append(slide);
            }

            return listBuilder.Build();
        }

        public (THIS, THIS) SplitAt(long idx)
        {
            var factory = GetArrayFactory();
            var empty = factory
                .NewBuilder()
                .Build();

            if (idx <= 0)
            {
                return (empty, This);
            }
            
            if (idx >= Length)
            {
                return (This, empty);
            }

            var firstStart = GetPhysicalIdx(0);
            var secondStart = GetPhysicalIdx(idx);

            return (
                factory.From(array, firstStart, idx, step),
                factory.From(array, secondStart, Length - idx, step)
            );
        }

        public void Deconstruct(out A first, out A second)
        {
            HeadOption.GetIfPresent(out first);
            Tail.HeadOption.GetIfPresent(out second);
        }

        public void Deconstruct(out A first, out A second, out A third)
        {
            Deconstruct(out first, out second);
            Tail.Tail.HeadOption.GetIfPresent(out third);
        }

        // protected members

        protected readonly A[] array;
        protected readonly long startIndex;
        protected readonly int step;

        protected internal ArrayBase(A[] array, long startIndex, long length, int step = 1)
        {
            this.array = array;
            this.startIndex = startIndex;
            this.step = step;
            Length = length;
        }

        protected A GetItemAt(long logicalIdx) => array[GetPhysicalIdx(logicalIdx)];
        protected long GetLogicalIdx(Index index) => index.IsFromEnd ? Length - 1 - index.Value : index.Value;
        protected long GetPhysicalIdx(long logicalIdx) => startIndex + step * logicalIdx;

        protected abstract ArrayBaseFactory GetArrayFactory();

        protected override IFactory<A, THIS> GetFactory() => GetArrayFactory();

        public abstract class ArrayBaseFactory : FactoryBase<A, THIS>
        {
            public virtual THIS From(A[] array) =>
                array != null ? From(array, 0, array.Length, 1) : NewBuilder().Build();

            public THIS From(A[] array, long startIndex, long length, int step)
            {
                if (array == null || array.LongLength == 0 || startIndex < 0 || length > array.LongLength)
                {
                    return NewBuilder().GetEmpty();
                }

                return InnerFrom(array, startIndex, length, step);
            }

            protected abstract THIS InnerFrom(A[] array, long startIndex, long length, int step);
        }

        // private members

        private long GetLowestPhysicalIdx() => step >= 0 ? startIndex : startIndex - Length + 1;
        private long GetHighestPhysicalIdx() => step >= 0 ? startIndex + Length - 1 : startIndex;

        IArray<A> IArray<A>.Init => Init;

        IArray<A> IArray<A>.Tail => Tail;

        IIndexable<A> IIndexable<A>.Tail => Tail;

        (IArray<A>, IArray<A>) IArray<A>.SplitAt(long idx) => SplitAt(idx);

        IIterable<IArray<A>> IArray<A>.Slide(long size, long nextStep) => Slide(size, nextStep).As<IArray<A>>();

        IArray<(A, Int)> IArray<A>.ZipWithIndex() =>
            (IArray<(A, Int)>)IndexableUtil.ZipWithIndex(this, GetFactory<(A, Int)>());

        IIndexable<(A, Int)> IIndexable<A>.ZipWithIndex() =>
            (IIndexable<(A, Int)>)IndexableUtil.ZipWithIndex(this, GetFactory<(A, Int)>());
    }
}
