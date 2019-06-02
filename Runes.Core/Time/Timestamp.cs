using System;

namespace Runes.Time
{
    public static class Timestamp
    {
        public static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static long GetUnixTimestampFrom(DateTimeOffset localizedDateTime)
        {
            var utc = localizedDateTime.ToUniversalTime();
            return (long) utc.Subtract(UnixEpoch).TotalSeconds;
        }
    }
}
