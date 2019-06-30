using System;

namespace Runes.Async.Jobs
{
    public interface IJob : IObservable<IJobStatus>
    {
        string Id { get; }

        Func<Action<Knowable<long>, int>, object> Task { get; }

        IJobStatus Status { get; set; }
    }
}
