using Runes.Async.Jobs;
using Runes.Diagnostic;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using static Runes.Predef;

namespace Runes.Async
{
    public sealed class Worker
    {
        public bool IsDone { get; private set; } = false;

        public void Notify()
        {
            runningMutex.Set();
        }

        public async Task StopAndWait()
        {
            stopping = true;

            Start();

            runningMutex.Set();

            await WorkToDo;

            IsDone = true;
        }

        public void Start()
        {
            startMutex.Set();
        }

        #region Builder type

        public sealed class Builder : IBuilder<Worker>
        {
            public static Builder New(ConcurrentQueue<IJob> jobsQueue) => new Builder(jobsQueue);

            public Worker Build() =>
                new Worker(
                    jobsQueue,
                    autoStart,
                    onPausedCallback,
                    onStartedCallback,
                    onStoppedCallback,
                    onStartJobCallback,
                    onFinishJobCallback
                );

            public Builder WithAutoStart(bool value)
            {
                autoStart = value;
                return this;
            }

            public Builder WithOnPausedCallback(Action callback)
            {
                onPausedCallback = Some(callback);
                return this;
            }

            public Builder WithOnStartedCallback(Action callback)
            {
                onStartedCallback = Some(callback);
                return this;
            }

            public Builder WithOnStoppedCallback(Action callback)
            {
                onStoppedCallback = Some(callback);
                return this;
            }

            public Builder WithOnStartJobCallback(Action<IJob> callback)
            {
                onStartJobCallback = Some(callback);
                return this;
            }

            public Builder WithOnFinishJobCallback(Action<IJob> callback)
            {
                onFinishJobCallback = Some(callback);
                return this;
            }

            // private members

            private readonly ConcurrentQueue<IJob> jobsQueue;
            private bool autoStart = false;
            private Option<Action> onPausedCallback = None<Action>();
            private Option<Action> onStartedCallback = None<Action>();
            private Option<Action> onStoppedCallback = None<Action>();
            private Option<Action<IJob>> onStartJobCallback = None<Action<IJob>>();
            private Option<Action<IJob>> onFinishJobCallback = None<Action<IJob>>();

            private Builder(ConcurrentQueue<IJob> jobsQueue) => this.jobsQueue = jobsQueue;
        }

        public sealed class WorkerSettings
        {
            public bool AutoStart { get; } = false;
            public Option<Action> OnPausedCallback { get; } = None<Action>();
            public Option<Action> OnStartedCallback { get; } = None<Action>();
            public Option<Action> OnStoppedCallback { get; } = None<Action>();
            public Option<Action<IJob>> OnStartJobCallback { get; } = None<Action<IJob>>();
            public Option<Action<IJob>> OnFinishJobCallback { get; } = None<Action<IJob>>();

            public WorkerSettings(ConcurrentQueue<IJob> jobsQueue) => JobsQueue = jobsQueue;

            internal ConcurrentQueue<IJob> JobsQueue { get; }
        }

        #endregion

        // private members

        private bool stopping = false;
        private readonly ManualResetEvent runningMutex, startMutex;
        private readonly Option<Action> onPausedCallback;
        private readonly Option<Action> onStartedCallback;
        private readonly Option<Action> onStoppedCallback;
        private readonly Option<Action<IJob>> onStartJobCallback;
        private readonly Option<Action<IJob>> onFinishJobCallback;

        private ConcurrentQueue<IJob> JobsQueue { get; }
        private Task WorkToDo { get; }

        private Worker(
            ConcurrentQueue<IJob> jobsQueue,
            bool autoStart,
            Option<Action> onPausedCallback,
            Option<Action> onStartedCallback,
            Option<Action> onStoppedCallback,
            Option<Action<IJob>> onStartJobCallback,
            Option<Action<IJob>> onFinishJobCallback
        ) {
            JobsQueue = jobsQueue;
            this.onPausedCallback = onPausedCallback;
            this.onStartedCallback = onStartedCallback;
            this.onStoppedCallback = onStoppedCallback;
            this.onStartJobCallback = onStartJobCallback;
            this.onFinishJobCallback = onFinishJobCallback;

            startMutex = new ManualResetEvent(false);
            startMutex.Reset();

            runningMutex = new ManualResetEvent(false);
            runningMutex.Reset();

            WorkToDo = CreateTask(autoStart);
        }

        private Task CreateTask(bool autoStart) => Task.Factory.StartNew(() =>
        {
            if (!autoStart)
            {
                startMutex.WaitOne();
            }

            onStartedCallback.Foreach(cb => cb());

            while (!stopping)
            {
                runningMutex.Reset();

                while (!stopping && JobsQueue.TryDequeue(out var job))
                {
                    DoWork(job);
                }

                if (!stopping)
                {
                    onPausedCallback.Foreach(cb => cb());
                    runningMutex.WaitOne();
                }
            }

            onStoppedCallback.Foreach(cb => cb());
        });

        private void DoWork(IJob job)
        {
            if (job.Status == ReadyToRun.Value)
            {
                job.Status = Running.Value;

                onStartJobCallback.Foreach(cb => cb(job));

                Func<Try<object>> tryTask = TryFunc(() => job.Execute());

                var tryTaskWithTimeAnalysis = TimeAnalyzer.WithTimeRange(tryTask);

                var (result, timeRange) = tryTaskWithTimeAnalysis();

                job.Status = DoneWithResult.Create(result, timeRange);

                onFinishJobCallback.Foreach(cb => cb(job));
            }
        }
    }
}
