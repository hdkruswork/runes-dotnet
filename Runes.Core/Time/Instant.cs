using Runes.Math;
using System;

using static Runes.Predef;

namespace Runes.Time
{
    public sealed class Instant : Ordered<Instant>
    {
        public static readonly Ordering<Instant> Ordering = OrderingBy<Instant>();

        public static readonly Instant UtcUnixEpoch = new Instant(0);

        public static Instant From(Int ticks) => new Instant(ticks);

        public static Instant From(int year, int month, int day, int hour, int minute, int second, long ticks) =>
            From(new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero).AddTicks(ticks));

        public static Instant From(int year, int month, int day, int hour, int minute, int second) =>
            From(new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero));

        public static Instant From(int year, int month, int day) => From(year, month, day, 0, 0, 0);

        public static Instant From(DateTimeOffset dateTimeOffset)
        {
            var utc = dateTimeOffset.ToUniversalTime();
            var ticks = utc >= TimestampUtils.UnixEpoch
                ? (utc - TimestampUtils.UnixEpoch).Ticks
                : -(TimestampUtils.UnixEpoch - utc).Ticks;

            return From(ticks);
        }

        public static Instant operator +(Instant instant, TimeInterval interval) => instant.Add(interval);

        public static Instant operator -(Instant instant, TimeInterval interval) => instant.Subtract(interval);

        public static TimeRange operator *(Instant i1, Instant i2) => i1.To(i2);

        public static TimeRange operator *(Instant instant, TimeInterval interval) => instant.To(instant + interval);

        public static bool operator ==(Instant x, Instant y) => x.EqualsTo(y);

        public static bool operator !=(Instant x, Instant y) => x.NonEqualsTo(y);

        public static bool operator <(Instant x, Instant y) => x.GreaterThan(y);

        public static bool operator >(Instant x, Instant y) => x.LesserThan(y);

        public static bool operator <=(Instant x, Instant y) => x.GreaterEqualsThan(y);

        public static bool operator >=(Instant x, Instant y) => x.LesserEqualsThan(y);

        public static TimeInterval operator -(Instant instant1, Instant instant2) => instant1.Difference(instant2);

        public Int Ticks { get; }

        public Instant Add(TimeInterval interval) => new Instant(Ticks + interval.InTicks);

        public override int CompareTo(Instant other) => Ticks.CompareTo(other.Ticks);

        public TimeInterval Difference(Instant other)
        {
            return TimeInterval.FromTicks(Ticks - other.Ticks);
        }

        public Instant Subtract(TimeInterval interval) => new Instant(Ticks - interval.InTicks);

        public Int ToSeconds() => Ticks / TimeInterval.TicksInASecond;

        public Int ToTimestamp() => ToSeconds();

        public DateTimeOffset ToUtcDateTime() => TimestampUtils.UnixEpoch.AddTicks((long)Ticks);

        public DateTimeOffset ToLocalizedDateTime(TimeSpan offset) => new DateTimeOffset(ToUtcDateTime().Ticks, offset);

        public DateTimeOffset ToLocalizedDateTime(double offset) => ToLocalizedDateTime(TimeSpan.FromHours(offset));

        public void Deconstruct(out int year, out int month, out int day) =>
            Deconstruct(out _, out year, out month, out day);

        public void Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second) =>
            Deconstruct(out _, out year, out month, out day, out hour, out minute, out second);

        public void Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out long ticks) =>
            Deconstruct(out _, out year, out month, out day, out hour, out minute, out second, out ticks);

        public override bool Equals(object obj) => obj is Instant instant && EqualsTo(instant);

        public override int GetHashCode() => typeof(Instant).GetHashCode() ^ GetFieldsHashCode(Ticks);

        public override string ToString() => ToUtcDateTime().ToString();

        // private members

        private Instant(Int ticks) => Ticks = ticks;

        private void Deconstruct(out DateTimeOffset utcDateTime, out int year, out int month, out int day)
        {
            utcDateTime = ToUtcDateTime();

            year = utcDateTime.Year;
            month = utcDateTime.Month;
            day = utcDateTime.Day;
        }

        private void Deconstruct(out DateTimeOffset utcDateTime, out int year, out int month, out int day, out int hour, out int minute, out int second)
        {
            Deconstruct(out utcDateTime, out year, out month, out day);

            hour = utcDateTime.Hour;
            minute = utcDateTime.Minute;
            second = utcDateTime.Second;
        }

        private void Deconstruct(out DateTimeOffset utcDateTime, out int year, out int month, out int day, out int hour, out int minute, out int second, out long ticks)
        {
            Deconstruct(out utcDateTime, out year, out month, out day, out hour, out minute, out second);

            var nearDate = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero);

            ticks = utcDateTime.Ticks - nearDate.Ticks;
        }
    }
}
