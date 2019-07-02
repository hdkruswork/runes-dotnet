using System;
using static Runes.Math.Predef;

namespace Runes.Time
{
    public sealed class TimeRange : IEquatable<TimeRange>
    {
        public static readonly TimeRange Empty = new TimeRange(Instant.UtcUnixEpoch, Instant.UtcUnixEpoch);

        public static TimeRange Create(Instant from, Instant nonInclusiveTo) =>
            from < nonInclusiveTo
                ? new TimeRange(from, nonInclusiveTo)
                : Empty;

        public static TimeRange Create(Instant instant, TimeInterval interval)
        {
            var instant2 = instant + interval;

            var ord = Instant.Ordering;
            var (from, to) = MinMax(ord, instant, instant2);

            return Create(from, to);
        }

        public Instant From { get; }

        public Instant To { get; }

        public bool IsEmpty => !NonEmpty;

        public bool NonEmpty => (To - From).NonEmpty;

        public TimeInterval Interval => To - From;

        public bool Contains(Instant instant) => From <= instant && instant < To;

        public bool Contains(TimeRange other) => From <= other.From && other.To <= To;

        public (TimeRange, TimeRange) Difference(TimeRange other)
        {
            var left = Empty;
            var right = Empty;

            if (Contains(other.From) && From < other.From)
            {
                left = Create(From, other.From);
            }
            if (Contains(other.To) && other.To < To)
            {
                right = Create(other.To, To);
            }

            return (left, right);
        }

        public bool Equals(TimeRange other) => From == other.From && To == other.To;

        public TimeRange Intersection(TimeRange other)
        {
            if (Contains(other.From))
            {
                return Create(other.From, Min(Instant.Ordering, To, other.To));
            }
            if (other.Contains(From))
            {
                return Create(From, Min(Instant.Ordering, To, other.To));
            }

            return Empty;
        }

        public bool Overlaps(TimeRange other) => Contains(other.From) || other.Contains(From);

        public (TimeRange, TimeRange) Partition(Instant instant)
        {
            if (instant <= From)
            {
                return (Empty, this);
            }
            if (To <= instant)
            {
                return (this, Empty);
            }

            return (Create(From, instant), Create(instant, To));
        }

        public bool UnionIfPossible(TimeRange other, out TimeRange union)
        {
            union = Empty;
            if (Overlaps(other) || From == other.To || other.From == To)
            {
                var ord = Instant.Ordering;
                union = Create(
                    Min(ord, From, other.From),
                    Max(ord, To, other.To)
                );
            }

            return union.NonEmpty;
        }

        // private members

        private TimeRange(Instant from, Instant nonInclusiveTo)
        {
            From = from;
            To = nonInclusiveTo;
        }
    }
}
