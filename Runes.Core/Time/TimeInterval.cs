﻿using Runes.Math;
using System;

namespace Runes.Time
{
    public sealed class TimeInterval
    {
        public const int TicksInASecond = 10_000_000;
        public static readonly Int TicksInAMinute = ((Int)TicksInASecond) * 60;
        public static readonly Int TicksInAnHour = TicksInAMinute * 60;
        public static readonly Int TicksInADay = TicksInAnHour * 24;
        public static readonly Int TicksInAWeek = TicksInADay * 7;

        public static implicit operator TimeInterval(TimeSpan span) => FromTicks(span.Ticks);

        public static explicit operator TimeSpan(TimeInterval interval) => TimeSpan.FromTicks((long) interval.Ticks);

        public static TimeInterval operator +(TimeInterval ti1, TimeInterval ti2) => FromTicks(ti1.Ticks + ti2.Ticks);

        public static TimeInterval operator -(TimeInterval ti1, TimeInterval ti2) => FromTicks(ti1.Ticks - ti2.Ticks);

        public static TimeInterval FromTicks(Int ticks) => new TimeInterval(ticks);

        public static TimeInterval FromWeeks(Rational weeks) => new TimeInterval((weeks * TicksInAWeek).WholePart);

        public static TimeInterval FromDays(Rational days) => new TimeInterval((days * TicksInADay).WholePart);

        public static TimeInterval FromHours(Rational hours) => new TimeInterval((hours * TicksInAnHour).WholePart);

        public static TimeInterval FromMinutes(Rational minutes) => new TimeInterval((minutes * TicksInAMinute).WholePart);

        public static TimeInterval FromSeconds(Rational seconds) => new TimeInterval((seconds * TicksInASecond).WholePart);

        public bool IsEmpty => !NonEmpty;

        public bool NonEmpty => Ticks != 0;

        public Int Ticks { get; }

        public Rational InWeeks => InDays / 7;

        public Rational InDays => InHours / 24;

        public Rational InHours => InMinutes / 60;

        public Rational InMinutes => InSeconds / 60;

        public Rational InSeconds => Ticks / TicksInASecond;

        private TimeInterval(Int ticks) => Ticks = ticks;
    }
}