using System;
using System.Collections.Generic;
using System.Text;

using static Runes.Predef;

namespace Runes.Collections
{
    public sealed class MutableArray<A> : ArrayBase<A, MutableArray<A>>, IMutableArray<A, MutableArray<A>>
    {
        public static readonly MutableArray<A> Empty = new MutableArray<A>(new A[0], 0, 0, 0);

        public static implicit operator MutableArray<A>(A[] array) => MArray(array);
        public static implicit operator A[](MutableArray<A> mArray) => mArray.array;

        public static MutableArray<A> CreateFrom(A[] array) => MArray(array, 0, array.LongLength, 1);
        public static MutableArray<A> CreateArrayFrom(ITraversable<A> traversable) =>
            CreateArrayFrom(traversable, CreateFrom);

        public new A this[long idx]
        {
            get => GetItemAt(idx);
            set => array[GetPhysicalIdx(idx)] = value;
        }

        public MutableArray<B> FlatMap<B>(Func<A, ICollection<B>> f) =>
            FlatMap(f, MutableArray<B>.CreateFrom);
        public MutableArray<B> Map<B>(Func<A, B> f) =>
            Map(f, MutableArray<B>.CreateFrom);
        public (MutableArray<X>, MutableArray<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) =>
            Unzip(toPairFunc, MutableArray<X>.CreateFrom, MutableArray<Y>.CreateFrom);
        public MutableArray<(A, B)> Zip<B>(ICollection<B> other) =>
            Zip(other, MutableArray<(A, B)>.CreateFrom);
        public MutableArray<(A, int)> ZipWithIndex() =>
            ZipWithIndex(MutableArray<(A, int)>.CreateFrom);

        // constructor

        internal MutableArray(A[] array, long startIndex, long length, int step = 1) : base(array, startIndex, length, step) { }

        // protected members

        protected override IArray<B> ArrayFlatMap<B>(Func<A, ICollection<B>> f) => FlatMap(f);
        protected override IArray<B> ArrayMap<B>(Func<A, B> f) => Map(f);
        protected override (IArray<X>, IArray<Y>) ArrayUnzip<X, Y>(Func<A, (X, Y)> toPairFunc) => Unzip(toPairFunc);
        protected override IArray<(A, B)> ArrayZip<B>(ICollection<B> other) => Zip(other);
        protected override IArray<(A, int)> ArrayZipWithIndex() => ZipWithIndex();

        protected override MutableArray<A> GetEmptyArray() => Empty;

        protected override MutableArray<A> CreateArray(A[] array, long startIndex, long length, int step) =>
            new MutableArray<A>(array, startIndex, length, step);

        protected override MutableArray<A> FromList(List<A> list) =>
            CreateArrayFrom(list, array => CreateArray(array, 0, array.LongLength, 1));

        // private members

        IMutableArray<A> IMutableArray<A>.Tail => Tail;
        IMutableArray<A> IMutableArray<A>.Init => Init;

        IMutableArray<A> IMutableArray<A>.Reversed() => Reversed();
        (IMutableArray<A>, IMutableArray<A>) IMutableArray<A>.Split(long index) => Split(index);
        ITraversable<IMutableArray<A>> IMutableArray<A>.Sliding(int size, int step) => Sliding(size, step).As<IMutableArray<A>>();
    }
}
