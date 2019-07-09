using Runes.Time;
using System;
using static Runes.Predef;

namespace Runes.Async
{
    public interface IProgress : IObservable<IProgress>
    {
        int MaxSteps {  get; }

        int Steps { get; }

        Option<TimeInterval> ETA { get; }

        string CurrentStatus { get; }

        IProgressSource Source { get; }
    }

    public interface IProgressSource
    {
        void Step(string status = null, TimeInterval eta = null);
    }

    public sealed class Progress : Observable<IProgress, Progress>, IProgress
    {
        public static readonly IProgressSource EmptySource = new ProgressSource((s, eta) => { });

        public static Progress Done()
        {
            var progress = Create(1);
            progress.Source.Step(eta: 0.Seconds());

            return progress;
        }

        public static Progress Create(int maxSteps) =>
            new Progress(maxSteps, string.Empty, None<TimeInterval>());

        public static Progress Create(int maxSteps, string initialStatus) =>
            new Progress(maxSteps, initialStatus, None<TimeInterval>());

        public static Progress Create(int maxSteps, TimeInterval eta) =>
            new Progress(maxSteps, string.Empty, Some(eta));

        public static Progress Create(int maxSteps, string initialStatus, TimeInterval eta) =>
            new Progress(maxSteps, initialStatus, Some(eta));

        public int MaxSteps { get; }

        public int Steps { get; private set; }

        public Option<TimeInterval> ETA { get; private set; }

        public string CurrentStatus { get; private set; }

        public IProgressSource Source { get; }

        // private members

        private Progress(int maxSteps, string initialStatus, Option<TimeInterval> eta)
        {
            MaxSteps = Max(1, maxSteps);
            Steps = 0;
            ETA = eta;
            CurrentStatus = initialStatus ?? string.Empty;

            Source = new ProgressSource(OnStep);
        }

        private void OnStep(string status, TimeInterval eta)
        {
            if (Steps < MaxSteps)
            {
                Steps += 1;
            }
            CurrentStatus = status ?? string.Empty;
            ETA = Option(eta);

            NotifyAll(this);
        }

        private sealed class ProgressSource : IProgressSource
        {
            private readonly Action<string, TimeInterval> onStep;

            public ProgressSource(Action<string, TimeInterval> onStep)
            {
                this.onStep = onStep;
            }

            public void Step(string status = null, TimeInterval eta = null) => onStep(status, eta);
        }
    }
}
