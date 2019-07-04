using Runes.Time;

using static Runes.Predef;

namespace Runes.Async.Jobs
{
    public interface IJobStatus
    {
        bool IsDone { get; }
        bool IsRunning {  get; }
    }

    public interface IProgressive : IJobStatus
    {
        Knowable<long> ETA { get; }

        int Progress {  get; }
    }

    public interface IUnresolved : IJobStatus
    {
    }

    public interface IReadyToRun : IUnresolved, IProgressive
    {
    }

    public interface IRunning : IUnresolved
    {
    }

    public interface IDone : IProgressive
    {
        TimeRange TimeRange { get; }

        Try<Unit> Result { get; }
    }

    public interface IDoneWithResult : IDone
    {
        new Try<object> Result {  get; }

        Try<Unit> IDone.Result => Result.Map(_ => Unit());
    }
}
