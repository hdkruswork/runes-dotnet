using System;

namespace Runes.Async.Jobs
{
    public sealed class Job: Observable<IJobStatus>, IJob
    {
        public static Job Create(string id, Func<object> task) => Create(id, id, _ => task());

        public static Job Create(string id, Func<Action<Knowable<long>, int>, object> task) => Create(id, id, task);

        public static Job Create(string id, string name, Func<object> task) => new Job(id, name, _ => task());

        public static Job Create(string id, string name, Func<Action<Knowable<long>, int>, object> task) => new Job(id, name, task);

        public string Id { get; }

        public string Name { get; }

        public Func<Action<Knowable<long>, int>, object> Task { get; }

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

        public override bool Equals(object obj) => obj is Job job && Equals(Id, job.Id);

        public override int GetHashCode() => typeof(Job).GetHashCode() ^ Id.GetHashCode();

        public override string ToString() => $"Job({(Equals(Id, Name) ? Id : $"{Id} - {Name}")}:{Status})";

        internal Job (string id, string name, Func<Action<Knowable<long>, int>, object> task)
        {
            Id = id;
            Name = name;
            Task = task;

            status = ReadyToRun.Value;
        }

        private IJobStatus status;
    }
}
