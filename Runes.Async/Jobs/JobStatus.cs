using Runes.Time;
using static Runes.Predef;

namespace Runes.Async.Jobs
{
    public abstract class Unresolved : IUnresolved
    {
        public virtual bool IsDone => false;

        public abstract bool IsRunning { get; }

        protected private Unresolved() { }
    }

    public sealed class ReadyToRun : Unresolved, IReadyToRun
    {
        public static readonly ReadyToRun Value = new ReadyToRun();

        public override bool IsRunning => false;

        public override string ToString() => "Ready";

        private ReadyToRun() { }
    }

    public class Running : Unresolved, IRunning
    {
        public static readonly Running Value = new Running();

        internal Running() { }

        public override bool IsRunning => true;

        public override string ToString() => "Running";
    }

    public class Done : IDone
    {
        public static Done Create(Try<Unit> result, TimeRange timeRange) => new Done(result, timeRange);

        public IProgress Progress => Async.Progress.Done();

        public bool IsDone => true;

        public bool IsRunning => false;

        public Try<Unit> Result { get; }

        public TimeRange TimeRange { get; }

        public override bool Equals(object obj) => obj is IDone;

        public override int GetHashCode() => typeof(IDone).GetHashCode() ^ 13;

        public override string ToString() => "Done";

        protected private Done(Try<Unit> result, TimeRange timeRange)
        {
            Result = result;
            TimeRange = timeRange;
        }
    }

    public sealed class DoneWithResult : Done, IDoneWithResult
    {
        public static DoneWithResult Create(Try<object> result, TimeRange timeRange) => new DoneWithResult(result, timeRange);

        public new Try<object> Result { get; }

        private DoneWithResult(Try<object> result, TimeRange timeRange) : base(result.Map(_ => Unit()), timeRange) =>
            Result = result;
    }
}
