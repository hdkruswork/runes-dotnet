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
    }

    public abstract class IterableBase<A, THIS> : CollectionBase<A, THIS>, IIterable<A, THIS>
        where THIS : IterableBase<A, THIS>
    {
        public abstract Option<A> HeadOption { get; }

        public abstract THIS Tail { get; }

        public override bool IsEmpty => HeadOption.IsEmpty;

        public override bool Contains(A item) => CollectionHelper.Contains(this, item);

        public virtual bool Correspond<B>(IIterable<B> other, Func<A, B, bool> f) => CollectionHelper.Correspond(this, other, f);

        public THIS Drops(Int count) => CollectionHelper.Drops<A, THIS>(This, count);

        public THIS DropsWhile(Func<A, bool> p) => DropsWhile(p, out _, true);

        public THIS DropsWhileNot(Func<A, bool> p) => DropsWhile(p, out _, false);

        public THIS DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped, true);

        public THIS DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped, false);

        public bool Exists(Func<A, bool> p) => Exists(p, true);

        public bool ExistsNot(Func<A, bool> p) => Exists(p, false);

        public virtual bool ForAll(Func<A, bool> p) => CollectionHelper.ForAll(this, p);

        public virtual That FoldLeft<That>(That initialValue, Func<That, A, That> f) =>
            CollectionHelper.FoldLeft(this, initialValue, f);

        public virtual That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f) =>
            CollectionHelper.FoldLeftWhile(this, initialValue, p, f);

        public Unit Foreach(Action<A> action) => 
            CollectionHelper.Foreach(this, action);

        public Unit ForeachWhile(Func<A, bool> p, Action<A> action) =>
            CollectionHelper.ForeachWhile(this, p, action);

        public virtual IEnumerator<A> GetEnumerator() => CollectionHelper.GetEnumerator(this);

        public virtual (THIS, THIS) Partition(Func<A, bool> p) => CollectionHelper.Partition(this, p, GetFactory());

        public virtual THIS Take(Int count) => CollectionHelper.Take(This, count, GetFactory());

        public THIS TakeWhile(Func<A, bool> p) => TakeWhile(p, true);

        public THIS TakeWhileNot(Func<A, bool> p) => TakeWhile(p, false);

        public That To<That>(IFactory<A, That> factory) where That : IIterable<A> => CollectionHelper.To(this, factory);

        public Array<A> ToArray() => To(Array<A>.Factory);

        public List<A> ToList() => CollectionHelper.ToList(this);

        public A[] ToMutableArray() => CollectionHelper.ToMutableArray(this);

        public Set<A> ToSet() => CollectionHelper.ToSet(this);

        public Stream<A> ToStream() => CollectionHelper.ToStream(this);

        // inner types

        public abstract class Builder<BB> : IIterableBuilder<A, THIS, BB> where BB : Builder<BB>
        {
            public abstract BB Append(A item);

            public abstract THIS Build();

            public abstract THIS GetEmpty();
        }

        // protected members

        protected That As<B, That>(IFactory<B, That> factory) where That : IIterable<B> =>
            CollectionHelper.As(this, factory);

        protected That Collect<B, That>(Func<A, Option<B>> f, IFactory<B, That> factory) where That : IIterable<B> =>
            CollectionHelper.Collect(this, f, factory);

        protected virtual THIS DropsWhile(Func<A, bool> p, out Int dropped, bool isAffirmative) =>
            CollectionHelper.DropsWhile(This, p, out dropped, isAffirmative);

        protected virtual bool Exists(Func<A, bool> p, bool isAffirmative) =>
            CollectionHelper.Exists(this, p, isAffirmative);

        protected override THIS Filter(Func<A, bool> p, bool isAffirmative) =>
            CollectionHelper.Filter(This, p, isAffirmative, GetFactory());

        protected That FlatMap<B, That>(Func<A, IIterable<B>> f, IFactory<B, That> factory) where That : IIterable<B> =>
            CollectionHelper.FlatMap(this, f, factory);

        protected abstract IFactory<A, THIS> GetFactory();

        protected abstract IFactory<B, IIterable<B>> GetFactory<B>();

        protected That Map<B, That>(Func<A, B> f, IFactory<B, That> factory) where That : IIterable<B> =>
            CollectionHelper.Map(this, f, factory);

        protected virtual THIS TakeWhile(Func<A, bool> p, bool isAffirmative) =>
            CollectionHelper.TakeWhile(This, p, isAffirmative, GetFactory());

        protected (Left, Right) Unzip<X, Y, Left, Right>(Func<A, (X, Y)> f, IFactory<X, Left> leftFactory, IFactory<Y, Right> rightFactory)
            where Left : IIterable<X> where Right : IIterable<Y> => CollectionHelper.Unzip(this, f, leftFactory, rightFactory);

        protected That Zip<B, That>(IIterable<B> other, IFactory<(A, B), That> factory)
            where That : IIterable<(A, B)> => CollectionHelper.Zip(this, other, factory);

        // Private members

        IIterable<A> IIterable<A>.Tail => Tail;

        IIterable<B> IIterable.As<B>() => As(GetFactory<B>());

        IIterable<B> IIterable<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f, GetFactory<B>());

        IIterable<B> IIterable<A>.FlatMap<B>(Func<A, IIterable<B>> f) => FlatMap(f, GetFactory<B>());

        IIterable<B> IIterable<A>.Map<B>(Func<A, B> f) => Map(f, GetFactory<B>());

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

        // explicit members

        IGrowable<A> ICollection<A, IGrowable<A>>.Filter(Func<A, bool> p) => Filter(p);
        IGrowable<A> ICollection<A, IGrowable<A>>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ICollection ICollection.Filter(Func<object, bool> p) => Filter((A a) => p(a));
        ICollection ICollection.FilterNot(Func<object, bool> p) => FilterNot((A a) => p(a));
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
