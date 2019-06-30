using Runes.Async.Jobs;
using Runes.Collections.Mutable;
using System;
using System.Threading.Tasks;

using static Runes.Predef;

namespace Runes.Async
{
    public sealed class ExecutersPool
    {
        private readonly Queue<IJob> jobsQueue = new Queue<IJob>();
        private readonly Task[] taskSlots;
        private readonly IJob[] runningJobs;

        public ReadOnlyArray<IJob> RunningJobs { get; }

        public int PoolSize { get; }

        public ExecutersPool() : this(Environment.ProcessorCount) { }

        public ExecutersPool(int poolSize)
        {
            if (poolSize <= 0)
            {
                poolSize = 1;
            }

            taskSlots = new Task[poolSize];
            runningJobs = new IJob[poolSize];
            RunningJobs = ReadOnlyArray(runningJobs);
            PoolSize = poolSize;
        }

        public string Execute(Func<object> task) =>
            Execute(task, null);

        public string Execute(Func<Action<Knowable<long>, int>, object> task) =>
            Execute(task, null);

        public string Execute(string id, Func<object> task) =>
            Execute(id, task, null);

        public string Execute(string id, Func<Action<Knowable<long>, int>, object> task) =>
            Execute(id, task, null);

        public string Execute(string id, string name, Func<object> task) =>
            Execute(id, name, task, null);

        public string Execute(string id, string name, Func<Action<Knowable<long>, int>, object> task) =>
            Execute(id, name, task, null);

        public string Execute(Func<object> task, Action<IJobStatus> statusChangeCallback) =>
            Execute(Guid.NewGuid().ToString(), task, statusChangeCallback);

        public string Execute(Func<Action<Knowable<long>, int>, object> task, Action<IJobStatus> statusChangeCallback) =>
            Execute(Guid.NewGuid().ToString(), task, statusChangeCallback);

        public string Execute(string id, Func<object> task, Action<IJobStatus> statusChangeCallback) =>
            Execute(id, id, task, statusChangeCallback);

        public string Execute(string id, Func<Action<Knowable<long>, int>, object> task, Action<IJobStatus> statusChangeCallback) =>
            Execute(id, id, task, statusChangeCallback);

        public string Execute(string id, string name, Func<object> task, Action<IJobStatus> statusChangeCallback) =>
            Execute(id, name, _ => task(), statusChangeCallback);

        public string Execute(
            string id,
            string name,
            Func<Action<Knowable<long>, int>, object> task,
            Action<IJobStatus> statusChangeCallback
        ) {
            var job = Job.Create(id, name, task);

            if (statusChangeCallback != null)
            {
                job.Subscribe(statusChangeCallback);
            }

            jobsQueue.Enqueue(job);

            StartEmptySlots();

            return id;
        }

        public void WaitForAll()
        {
            foreach (var task in taskSlots)
            {
                if (task != null)
                {
                    task.Wait();
                }
            }
        }

        // private members

        private void StartEmptySlots()
        {
            for (int i = 0; i < taskSlots.Length; i++)
            {
                CreateTaskInSlot(i);
            }
        }

        private void CreateTaskInSlot(int slot)
        {
            if (taskSlots[slot] == null)
            {
                taskSlots[slot] = Task.Factory.StartNew(() =>
                {
                    while (jobsQueue.Dequeue(out var job))
                    {
                        if (job.Status == ReadyToRun.Value)
                        {
                            job.Status = Running.Value;
                            runningJobs[slot] = job;

                            var result = Try(() => job.Task((eta, progress) => job.Status = RunningWithProgress.Create(eta, progress)));
                            if (result.GetIfSuccess(out var res))
                            {
                                job.Status = DoneWithResult.Create(res);
                            }
                            else if (result.GetIfFailure(out Exception ex))
                            {
                                job.Status = DoneWithResult.Create(ex);
                            }
                            runningJobs[slot] = null;
                        }
                    }

                    taskSlots[slot] = null;
                });
            }
        }
    }
}
