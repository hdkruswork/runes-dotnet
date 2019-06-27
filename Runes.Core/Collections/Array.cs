using System;
using System.Collections.Generic;
using System.Linq;
using Runes.Math;

namespace Runes.Collections
{
    public class Array<A> : ArrayBase<A, Array<A>>
    {
        public static readonly ArrayFactory Factory = new ArrayFactory();

        public static readonly Array<A> Empty = Factory.NewBuilder().Build();

        public static implicit operator Array<A>(A[] array) => Factory.From(array);

        public Array<B> As<B>() => As(Array<B>.Factory);

        public Array<B> Collect<B>(Func<A, Option<B>> f) => Collect(f, Array<B>.Factory);

        public Array<B> FlatMap<B>(Func<A, IIterable<B>> f) => FlatMap(f, Array<B>.Factory);

        public Array<B> Map<B>(Func<A, B> f) => Map(f, Array<B>.Factory);

        public (Array<X>, Array<Y>) Unzip<X, Y>(Func<A, (X, Y)> f) => Unzip(f, Array<X>.Factory, Array<Y>.Factory);

        public Array<(A, B)> Zip<B>(IIterable<B> other) => Zip(other, Array<(A, B)>.Factory);

        public Array<(A, Int)> ZipWithIndex() => IndexableUtil.ZipWithIndex(this, Array<(A, Int)>.Factory);

        // protected members

        protected override IFactory<B, IIterable<B>> GetFactory<B>() => Array<B>.Factory;

        protected override ArrayBaseFactory GetArrayFactory() => Factory;

        // protected private members

        protected private Array(A[] array, long startIndex, long length, int step = 1) : base(array, startIndex, length, step)
        {
        }

        // inner types

        public sealed class ArrayFactory : ArrayBaseFactory
        {
            public override IIterableBuilder<A, Array<A>> NewBuilder() => new ArrayBuilder();

            protected override Array<A> InnerFrom(A[] array, long startIndex, long length, int step) =>
                new Array<A>(array, startIndex, length, step);
        }

        private sealed class ArrayBuilder : Builder<ArrayBuilder>
        {
            private readonly LinkedList<A> list = new LinkedList<A>();

            public override ArrayBuilder Append(A item)
            {
                list.AddLast(item);
                return this;
            }

            public override Array<A> Build()
            {
                var array = list.ToArray();
                return new Array<A>(array, 0, array.LongCount());
            }

            public override Array<A> GetEmpty() => Empty;
        }
    }
}
