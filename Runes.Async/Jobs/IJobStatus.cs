namespace Runes.Async.Jobs
{
    public interface IJobStatus
    {
        bool IsDone { get; }
        bool IsRunning {  get; }
    }

    public interface IProgressive : IJobStatus
    {
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
    }

    public interface IDoneWithResult : IDone
    {
        object Result {  get; }
    }
}
