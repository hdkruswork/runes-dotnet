using System.Collections;

namespace Runes.Text
{
    public static class TextUtils
    {
        public static string Join(IEnumerable enumerable, string start, string separator, string ends)
        {
            var builder = new StringBuilder();
            var hasItems = false;

            builder.Append(start);
            foreach (var item in enumerable)
            {
                builder.Append(item);
                builder.Append(separator);
                hasItems = true;
            }
            if (hasItems)
            {
                builder.RemoveLast();
            }
            builder.Append(ends);

            return builder.ToString();
        }
    }
}
