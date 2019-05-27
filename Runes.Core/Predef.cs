using static Runes.Options;

namespace Runes
{
    public static class Predef
    {
        public static Option<A> As<A>(this object obj) => obj is A casted ? Some(casted) : None<A>();
    }
}
