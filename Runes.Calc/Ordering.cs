using Runes.Math;

using static Runes.Predef;

namespace Runes.Calc
{
    public static class Ordering
    {
        public static readonly Ordering<Int> Int = OrderingBy<Int>();
        public static readonly Ordering<Rational> Rational = OrderingBy<Rational>();
    }
}
