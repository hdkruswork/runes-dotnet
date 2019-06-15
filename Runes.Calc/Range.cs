using System;
using Runes.Collections;
using Runes.Math;

using SysCollections = System.Collections;
using SysGenCollections = System.Collections.Generic;

using static Runes.Predef;
using static Runes.Calc.Predef;

namespace Runes.Calc
{
    public sealed class Range : IArray<Int>, IEquatable<Range>
    {
        private int Increment { get; }

        public Int From { get; }
        public Int To { get; }

        public bool IsEmpty => Increment >= 0 ? From + Increment <= To : From + Increment >= To;
        public bool NonEmpty => !IsEmpty;

        public Option<Int> this[long idx]
        {
            get
            {
                var value = GetItemAt(idx);
                return Contains(value) ? Some(value) : None<Int>();
            }
        }

        public long Length => NonEmpty ? System.Math.Abs((long)(To - From)) : 0;
        public long Size => Length;

        public Option<Int> HeadOption => NonEmpty ? Some(From) : None<Int>();
        public Range Tail => NonEmpty ? Range(From + Increment, To, Increment) : this;

        public Range Init => NonEmpty ? Range(From, To - Increment, Increment) : this;
        public Option<Int> Rear => NonEmpty ? Some(To) : None<Int>();

        public IArray<B> As<B>() where B : class => this.ToArray().As<B>();

        public IArray<B> Collect<B>(Func<Int, Option<B>> f) => this.ToArray().Collect(f);

        public bool Contains(Int item)
        {
            if (IsEmpty)
            {
                return false;
            }

            var (min, max) = MinMax(From, To);

            return min <= item && item <= max;
        }

        public IArray<Int> Drops(int count) => this.ToArray().Drops(count);

        public IArray<Int> DropsWhile(Func<Int, bool> p) => this.ToArray().DropsWhile(p);

        public IArray<Int> DropsWhileNot(Func<Int, bool> p) => this.ToArray().DropsWhileNot(p);

        public IArray<Int> DropsWhile(Func<Int, bool> p, out int skipped) => this.ToArray().DropsWhile(p, out skipped);

        public IArray<Int> DropsWhileNot(Func<Int, bool> p, out int skipped) => this.ToArray().DropsWhileNot(p, out skipped);

        public bool Equals(Range other) => From == other.From && To == other.To && Increment == other.Increment;

        public override bool Equals(object obj) => obj is Range other && Equals(other);

        public bool Exist(Func<Int, bool> p)
        {
            foreach (var item in this)
            {
                if (p(item))
                {
                    return true;
                }
            }

            return false;
        }

        public IArray<Int> Filter(Func<Int, bool> p) => this.ToArray().Filter(p);

        public IArray<Int> FilterNot(Func<Int, bool> p) => this.ToArray().FilterNot(p);

        public override int GetHashCode() => GetFieldsHashCode(From, To, Increment);

        public IArray<Int> Take(int count) => this.ToArray().Take(count);

        public IArray<Int> TakeWhile(Func<Int, bool> p) => this.ToArray().TakeWhile(p);

        public IArray<Int> TakeWhileNot(Func<Int, bool> p) => this.ToArray().TakeWhileNot(p);

        public IArray<B> FlatMap<B>(Func<Int, ICollection<B>> f) => this.ToArray().FlatMap(f);

        public That FoldLeft<That>(That initialValue, Func<That, Int, That> f) => this.ToArray().FoldLeft(initialValue, f);

        public That FoldLeftWhile<That>(That initialValue, Func<That, Int, bool> p, Func<That, Int, That> f) =>
            this.ToArray().FoldLeftWhile(initialValue, p, f);

        public That FoldRight<That>(That initialValue, Func<That, Int, That> f) => this.ToArray().FoldRight(initialValue, f);

        public That FoldRightWhile<That>(That initialValue, Func<That, Int, bool> p, Func<That, Int, That> f) =>
            this.ToArray().FoldRightWhile(initialValue, p, f);

        public Unit For(Action<Int, long> action) => Unit(() =>
        {
            for (long idx = 0; idx < Length; idx++)
            {
                action(GetItemAt(idx), idx);
            }
        });

        public bool ForAll(Func<Int, bool> p)
        {
            foreach (var item in this)
            {
                if (!p(item))
                {
                    return false;
                }
            }

            return true;
        }

        public Unit Foreach(Action<Int> action) => Unit(() =>
        {
            foreach (var item in this)
            {
                action(item);
            }
        });

        public Unit ForeachWhile(Func<Int, bool> p, Action<Int> action) => Unit(() =>
        {
            foreach (var item in this)
            {
                if (!p(item))
                {
                    break;
                }

                action(item);
            }
        });

        public SysGenCollections.IEnumerator<Int> GetEnumerator()
        {
            for (long idx = 0; idx < Length; idx++)
            {
                yield return GetItemAt(idx);
            }
        }

        public IArray<B> Map<B>(Func<Int, B> f) => this.ToArray().Map(f);

        public Range Reversed() => Range(To, From, -Increment);

        public Range Sort(Ordering<Int> ord) => ord.Compare(From, To) <= 0 ? this : Reversed();

        public override string ToString() => NonEmpty ? $"[{From}, {To}]" : $"[,]";

        public (IArray<X>, IArray<Y>) Unzip<X, Y>(Func<Int, (X, Y)> toPairFunc) => this.ToArray().Unzip(toPairFunc);

        public IArray<(Int, B)> Zip<B>(ICollection<B> other) => this.ToArray().Zip(other);

        public IArray<(Int, int)> ZipWithIndex() => this.ToArray().ZipWithIndex();

        internal Range(Int from, Int to, int increment)
        {
            From = from;
            To = to;
            Increment = increment;
        }

        // private members

        private Int GetItemAt(long idx) => From + idx * Increment;

        IArray<Int> IArray<Int>.Init => Init;

        IIterable<Int> IIterable<Int>.Tail => Tail;

        ICollection<Int> ICollection<Int>.Tail => Tail;

        SysCollections.IEnumerator SysCollections.IEnumerable.GetEnumerator() => GetEnumerator();

        (IArray<Int>, IArray<Int>) IArray<Int>.Split(long index) => this.ToArray().Split(index);

        IIterable<IArray<Int>> IArray<Int>.Sliding(int size, int step) => this.ToArray().Sliding(size, step).As<IArray<Int>>();

        IIterable<B> IIterable<Int>.As<B>() => As<B>();

        IIterable<B> IIterable<Int>.Collect<B>(Func<Int, Option<B>> f) => Collect(f);

        IIterable<B> IIterable<Int>.FlatMap<B>(Func<Int, ICollection<B>> f) => FlatMap(f);

        IIterable<B> IIterable<Int>.Map<B>(Func<Int, B> f) => Map(f);

        IIterable<Int> IIterable<Int>.Reversed() => Reversed();

        IIterable<Int> IIterable<Int>.Sort(Ordering<Int> ord) => Sort(ord);

        (IIterable<X>, IIterable<Y>) IIterable<Int>.Unzip<X, Y>(Func<Int, (X, Y)> toPairFunc) => Unzip(toPairFunc);

        IIterable<(Int, B)> IIterable<Int>.Zip<B>(ICollection<B> other) => Zip(other);

        IIterable<(Int, int)> IIterable<Int>.ZipWithIndex() => ZipWithIndex();

        ICollection<B> ICollection<Int>.As<B>() => As<B>();

        ICollection<B> ICollection<Int>.Collect<B>(Func<Int, Option<B>> f) => Collect(f);

        ICollection<Int> ICollection<Int>.Drops(int count) => Drops(count);

        ICollection<Int> ICollection<Int>.DropsWhile(Func<Int, bool> p) => DropsWhile(p);

        ICollection<Int> ICollection<Int>.DropsWhileNot(Func<Int, bool> p) => DropsWhileNot(p);

        ICollection<Int> ICollection<Int>.DropsWhile(Func<Int, bool> p, out int skipped) => DropsWhile(p, out skipped);

        ICollection<Int> ICollection<Int>.DropsWhileNot(Func<Int, bool> p, out int skipped) => DropsWhileNot(p, out skipped);

        ICollection<Int> ICollection<Int>.Filter(Func<Int, bool> p) => Filter(p);

        ICollection<Int> ICollection<Int>.FilterNot(Func<Int, bool> p) => FilterNot(p);

        ICollection<B> ICollection<Int>.FlatMap<B>(Func<Int, ICollection<B>> f) => FlatMap(f);

        ICollection<B> ICollection<Int>.Map<B>(Func<Int, B> f) => Map(f);

        ICollection<Int> ICollection<Int>.Take(int count) => Take(count);

        ICollection<Int> ICollection<Int>.TakeWhile(Func<Int, bool> p) => TakeWhile(p);

        ICollection<Int> ICollection<Int>.TakeWhileNot(Func<Int, bool> p) => TakeWhileNot(p);

        (ICollection<X>, ICollection<Y>) ICollection<Int>.Unzip<X, Y>(Func<Int, (X, Y)> toPairFunc) => Unzip(toPairFunc);

        ICollection<(Int, B)> ICollection<Int>.Zip<B>(ICollection<B> other) => Zip(other);

        ICollection<(Int, int)> ICollection<Int>.ZipWithIndex() => ZipWithIndex();
    }
}
