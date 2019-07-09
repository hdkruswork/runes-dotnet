using System;
using System.Threading;

namespace Runes.Async.Jobs
{
    public sealed class Job: Observable<IJobStatus>, IJob
    {
        public static Job Create(IJobSettings settings, Func<IJobSettings, object> f) => new Job(settings, f);

        public IJobStatus Status
        {
            get {  return status; }
            set
            {
                if (!Equals(value, status))
                {
                    status = value;
                    NotifyAll(value);
                }
            }
        }

        public object Execute() => ExecuteFunc(Settings);

        public override string ToString() => $"Job({Status})";

        private Job (IJobSettings settings, Func<IJobSettings, object> f)
        {
            ExecuteFunc = f;
            Settings = settings;

            status = ReadyToRun.Value;
        }

        private IJobStatus status;

        private IJobSettings Settings { get; }

        private Func<IJobSettings, object> ExecuteFunc { get; }
    }

    public sealed class JobSettings : IJobSettings
    {
        public CancellationToken CancellationToken { get; set; }

        public IProgressSource ProgressSource { get; set; }

        public JobSettings()
        {
            CancellationToken = new CancellationTokenSource().Token;
            ProgressSource = Progress.EmptySource;
        }
    }
}
