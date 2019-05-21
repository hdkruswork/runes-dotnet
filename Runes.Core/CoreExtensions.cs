namespace Runes
{
    public static class CoreExtensions
    {
        public static bool Is<A>(this object obj, out A castedObj)
        {
            castedObj = default;
            var res = obj is A;
            if (res)
            {
                castedObj = (A)obj;
            }

            return res;
        }
    }
}
