using System.Threading;

namespace Runes.Async.Jobs
{
    public interface IJob : IObservable<IJobStatus>
    {
        IJobStatus Status { get; set; }

        object Execute();
    }

    public interface IJobSettings
    {
        CancellationToken CancellationToken { get; }

        IProgressSource ProgressSource { get; }
    }
}
