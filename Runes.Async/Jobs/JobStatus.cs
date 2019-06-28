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

        public int Progress => 0;

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

    public class RunningWithProgress : Running, IProgressive
    {
        public static RunningWithProgress Create(int progress) => new RunningWithProgress(progress);

        public int Progress { get; }

        public override bool IsDone => Progress >= 100;

        public bool Equals(RunningWithProgress other) => Progress == other.Progress;

        public override bool Equals(object obj) => obj is RunningWithProgress other && Equals(other);

        public override int GetHashCode() => typeof(RunningWithProgress).GetHashCode() ^ Progress.GetHashCode();

        public override string ToString() => $"Running({Progress}%)";

        internal RunningWithProgress(int progress) => Progress = progress;
    }

    public class Done : IDone
    {
        public static readonly Done Value = new Done();

        public int Progress => 100;

        public bool IsDone => true;

        public bool IsRunning => false;

        public override bool Equals(object obj) => obj is IDone;

        public override int GetHashCode() => typeof(IDone).GetHashCode() ^ 13;

        public override string ToString() => "Done";

        internal Done() { }
    }

    public sealed class DoneWithResult : Done, IDoneWithResult
    {
        public static DoneWithResult Create(object result) => new DoneWithResult(result);

        public object Result { get; }

        internal DoneWithResult(object result) => Result = result;
    }
}
