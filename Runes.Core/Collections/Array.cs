﻿using System;
using Runes.Math;

using static Runes.Predef;

namespace Runes.Collections
{
    public sealed class Array<A> : ArrayBase<A, Array<A>>
    {
        public static readonly Array<A> Empty = new Array<A>(new A[0], 0, 0, 0);

        public static implicit operator Array<A>(A[] array) => Array(array);

        public static Array<A> CreateFrom(A[] array) => new Array<A>(array, 0, array.LongLength);

        public static Array<A> CreateArrayFrom(IIterable<A> iterable) => CreateArrayFrom(iterable, CreateFrom);

        public Array<B> FlatMap<B>(Func<A, ICollection<B>> f) => FlatMap(f, Array<B>.CreateFrom);

        public Array<B> Map<B>(Func<A, B> f) => Map(f, Array<B>.CreateFrom);

        public override Array<A> Sort(Ordering<A> ord)
        {
            var arrayCopy = new A[Length];
            for (long idx = 0; idx < Length; idx++)
            {
                arrayCopy[idx] = GetItemAt(idx);
            }

            Quicksort<A>().Sort(MArray(arrayCopy), ord);

            return Array(arrayCopy);
        }

        public (Array<X>, Array<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) =>
            Unzip(toPairFunc, Array<X>.CreateFrom, Array<Y>.CreateFrom);
        
        public Array<(A, B)> Zip<B>(ICollection<B> other) => Zip(other, Array<(A, B)>.CreateFrom);

        public Array<(A, int)> ZipWithIndex() => ZipWithIndex(Array<(A, int)>.CreateFrom);

        // constructor

        internal Array(A[] array, long startIndex, long length, int step = 1) : base(array, startIndex, length, step) { }

        // protected members

        protected override IArray<B> ArrayFlatMap<B>(Func<A, ICollection<B>> f) => FlatMap(f);
        protected override IArray<B> ArrayMap<B>(Func<A, B> f) => Map(f);
        protected override (IArray<X>, IArray<Y>) ArrayUnzip<X, Y>(Func<A, (X, Y)> toPairFunc) => Unzip(toPairFunc);
        protected override IArray<(A, B)> ArrayZip<B>(ICollection<B> other) => Zip(other);
        protected override IArray<(A, int)> ArrayZipWithIndex() => ZipWithIndex();

        protected override Array<A> GetEmptyArray() => Empty;
        protected override Array<A> CreateArray(A[] arr, long start, long length, int stp) =>
            new Array<A>(arr, start, length, stp);

        protected override Array<A> FromList(List<A> list) =>
            CreateArrayFrom(list, array => CreateArray(array, 0, array.LongLength, 1));
    }
}
