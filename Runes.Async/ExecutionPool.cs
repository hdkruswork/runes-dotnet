using Runes.Async.Jobs;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static Runes.Predef;

namespace Runes.Async
{
    public sealed class ExecutionPool : Observable<(Option<IJob>, int)>
    {
        public static ExecutionPool Create(int poolSize) => new ExecutionPool(poolSize, new ConcurrentQueue<IJob>());

        public static ExecutionPool Create() => Create(Environment.ProcessorCount);

        public int PoolSize { get; }

        public void Execute(
            IJobSettings settings,
            Func<IJobSettings, object> f,
            Action<IJobStatus> statusChangeCallback
        ) {
            var job = Job.Create(settings, f);

            if (statusChangeCallback != null)
            {
                job.Subscribe(statusChangeCallback);
            }

            jobsQueue.Enqueue(job);

            workers.Foreach(w => w.Notify());
        }

        public void Start()
        {
            for (int slot = 0; slot < PoolSize; slot++)
            {
                var worker = Worker.Builder
                    .New(jobsQueue)
                    .WithAutoStart(true)
                    .WithOnStartJobCallback(job => OnJobStart(job, slot))
                    .WithOnFinishJobCallback(_ => OnJobFinish(slot))
                    .Build();

                workers[slot] = worker;
            }
        }

        public async Task StopAndWait()
        {
            var tasks = new Task[PoolSize];

            for (int slot = 0; slot < PoolSize; slot++)
            {
                tasks[slot] = workers[slot].StopAndWait();
                workers[slot] = null;
            }

            await Task.WhenAll(tasks);
        }

        public ExecutionPool CreateNew(int poolsize) => new ExecutionPool(poolsize, jobsQueue);

        // private members

        private readonly ConcurrentQueue<IJob> jobsQueue;
        private readonly Worker[] workers;

        private ExecutionPool(int poolSize, ConcurrentQueue<IJob> queue)
        {
            jobsQueue = queue;
            if (poolSize <= 0)
            {
                poolSize = 1;
            }

            PoolSize = poolSize;
            workers = new Worker[PoolSize];

            Start();
        }

        private void OnJobStart(IJob job, int slot) => NotifyAll((Some(job), slot));

        private void OnJobFinish(int slot) => NotifyAll((None<IJob>(), slot));
    }
}
