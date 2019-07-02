using Runes.Math;
using System;

namespace Runes.Time
{
    public static class TimestampUtils
    {
        public static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static long GetUnixTimestampFrom(DateTimeOffset localizedDateTime)
        {
            var utc = localizedDateTime.ToUniversalTime();
            return (long) utc.Subtract(UnixEpoch).TotalSeconds;
        }

        public static DateTimeOffset GetUtcDateTimeFrom(long timestamp) => UnixEpoch.AddSeconds(timestamp);

        public static DateTimeOffset GetLocalizedDateTimeFrom(long timestamp, TimeSpan offset)
        {
            var utcDateTime = GetUtcDateTimeFrom(timestamp).UtcDateTime;

            return new DateTimeOffset(utcDateTime, offset);
        }
    }
}
