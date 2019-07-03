using Runes.Time;
using System;

using static Runes.Predef;

namespace Runes.Diagnostic
{
    public class TimeAnalyzer
    {
        public static Func<A, B, C, (R, TimeRange)> WithTimeRange<A, B, C, R>(Func<A, B, C, R> f) =>
            (a, b, c) => {
                var starts = Now();
                var res = f(a, b, c);
                var ends = Now();

                return (res, (starts, ends));
            };

        public static Func<A, B, (R, TimeRange)> WithTimeRange<A, B, R>(Func<A, B, R> f) =>
            (a, b) => {
                var starts = Now();
                var res = f(a, b);
                var ends = Now();

                return (res, (starts, ends));
            };

        public static Func<A, (R, TimeRange)> WithTimeRange<A, R>(Func<A, R> f) =>
            a => {
                var starts = Now();
                var res = f(a);
                var ends = Now();

                return (res, (starts, ends));
            };

        public static Func<(R, TimeRange)> WithTimeRange<R>(Func<R> f) =>
            () => {
                var starts = Now();
                var res = f();
                var ends = Now();

                return (res, (starts, ends));
            };

        public static Func<A, B, C, TimeRange> WithTimeRange<A, B, C>(Action<A, B, C> action) =>
            (a, b, c) => {
                var starts = Now();
                action(a, b, c);
                var ends = Now();

                return (starts, ends);
            };

        public static Func<A, B, TimeRange> WithTimeRange<A, B>(Action<A, B> action) =>
            (a, b) => {
                var starts = Now();
                action(a, b);
                var ends = Now();

                return (starts, ends);
            };

        public static Func<A, TimeRange> WithTimeRange<A>(Action<A> action) =>
            a => {
                var starts = Now();
                action(a);
                var ends = Now();

                return (starts, ends);
            };

        public static Func<TimeRange> WithTimeRange(Action action) =>
            () => {
                var starts = Now();
                action();
                var ends = Now();

                return (starts, ends);
            };
    }
}
