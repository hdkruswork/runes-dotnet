using System;
using System.Collections.Generic;

using static Runes.Predef;

namespace Runes.Math
{
    public abstract class Ordering<A> : IComparer<A>
    {
        public static Ordering<A> By(Func<A, A, int> f) => new OrderingBy(f);

        public abstract int Compare(A x, A y);

        public bool Equals(A x, A y) => Compare(x, y) == 0;
        
        public bool GreaterThan(A x, A y) => Compare(x, y) > 0;
        
        public bool GreaterEqualsThan(A x, A y) => Compare(x, y) >= 0;
        
        public bool LesserThan(A x, A y) => Compare(x, y) < 0;
        
        public bool LesserEqualsThan(A x, A y) => Compare(x, y) <= 0;

        public B Max<B>(B x, B y) where B : A => GreaterEqualsThan(x, y) ? x : y;
        
        public B Min<B>(B x, B y) where B : A => LesserEqualsThan(x, y) ? x : y;

        public Ordering<B> On<B>(Func<B, A> f) => OrderingBy<B>((x, y) => Compare(f(x), f(y)));
        
        public Ordering<A> OrElse(Ordering<A> other) =>
            OrderingBy<A>((a, b) =>
            {
                var thisRes = Compare(a, b);
                return thisRes == 0 ? other.Compare(a, b) : thisRes;
            });

        public Ordering<A> OrElseBy<B>(Func<A, B> f, Ordering<B> ord) =>
            OrderingBy<A>((a, b) =>
            {
                var thisRes = Compare(a, b);
                return thisRes == 0 ? ord.Compare(f(a), f(b)) : thisRes;
            });

        public Ordering<A> Reverse() => OrderingBy<A>((a, b) => Compare(b, a));

        // inner classes

        private sealed class OrderingBy : Ordering<A>
        {
            private readonly Func<A, A, int> comparingFunc;

            public OrderingBy(Func<A, A, int> comparingFunc)
            {
                this.comparingFunc = comparingFunc;
            }

            public override int Compare(A x, A y) => comparingFunc(x, y);
        }
    }
}
