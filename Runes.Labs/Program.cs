using Runes.Async;
using Runes.Async.Jobs;
using Runes.Collections;
using Runes.Math;
using Runes.Security.Cryptography;
using Runes.Text;
using System;
using System.Threading;
using static Runes.Predef;

namespace Runes.Labs
{
    class Program
    {
        static void Main()
        {
            TestExecutionPool();

            Println("====================");

            TestStreams();

            Println("====================");
        }

        private static void TestExecutionPool()
        {
            var hexDecoder = HexDecoder.Object;
            var sha256 = Sha256Algorithm.Object;

            Println("Testing execution pool");

            Println($"Curr dir: {AppDomain.CurrentDomain.BaseDirectory}");

            var executorPool = ExecutionPool.Create();

            Println($"Pool size: {executorPool.PoolSize}");

            var consoleSyncObj = new object();
            Action<string, IJobStatus> updateStatus = (id, status) =>
            {
                lock (consoleSyncObj)
                {
                    switch (status)
                    {
                        case DoneWithResult done when done.Result is Success<object> success:
                            Println(
                                $"Job: {id} - Done with result: {success.Result} in {done.TimeRange.Interval.InMilliSeconds.WholePart} ms",
                                foreColor: ConsoleColor.Green
                            );
                            break;

                        case DoneWithResult done when done.Result is Failure<object> fail:
                            Println(
                                $"Job: {id} - Failed: {fail.Exception.Message} in {done.TimeRange.Interval.InMilliSeconds.WholePart} ms",
                                foreColor: ConsoleColor.Red
                            );
                            break;

                        default:
                            Println($"Job: {id} : {status}", foreColor: ConsoleColor.Yellow);
                            break;
                    }
                }
            };

            var firstTaskName = "1st Task";
            var firstJobCancellationSource = new CancellationTokenSource();
            var firstJobSettings = new JobSettings
            {
                CancellationToken = firstJobCancellationSource.Token,
                ProgressSource = Progress
                    .Create(maxSteps: 4)
                    .Subscribe(progress => {
                        var percent = ((double)progress.Steps) / progress.MaxSteps * 100;
                        var etaMessage = progress.ETA
                            .Map(eta => $" ETA: {eta.InMilliSeconds.WholePart} ms")
                            .GetOrElse(string.Empty);
                        var status = $"{firstTaskName} > {percent.ToString(".00")}%{etaMessage}";

                        Println(status, foreColor: ConsoleColor.Yellow);
                    })
                    .Source
            };

            executorPool.Execute(
                firstJobSettings,
                settings => {
                    settings.ProgressSource.Step(eta: 1500.Milliseconds());

                    Thread.Sleep(500);
                    settings.ProgressSource.Step(eta: 1000.Milliseconds());

                    settings.CancellationToken.ThrowIfCancellationRequested();

                    Thread.Sleep(500);
                    settings.ProgressSource.Step(eta: 500.Milliseconds());

                    settings.CancellationToken.ThrowIfCancellationRequested();

                    Thread.Sleep(500);
                    settings.ProgressSource.Step(eta: 0.Milliseconds());

                    return 120;
                },
                updateStatus.Curry(firstTaskName)
            );

            var secondTaskName = "2nd Task";
            var secondJobSetting = new JobSettings();

            executorPool.Execute(
                secondJobSetting,
                _ => {
                    Thread.Sleep(750);
                    return 480;
                },
                updateStatus.Curry(secondTaskName)
            );

            Thread.Sleep(750);
            firstJobCancellationSource.Cancel();

            executorPool.StopAndWait().GetAwaiter().GetResult();
        }

        private static void TestStreams()
        {
            static string ConjugateCorrespondVerb(bool affirmative) => affirmative ? "corresponds" : "DOESN'T correspond";

            // Factorial test

            static Stream<Int> factorial() =>
                Stream(1, () => factorial().Zip(StartStream(1)).Map(p => p.Item1 * p.Item2));

            var expectedFactorial = Stream<Int>(1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880);

            var factCorresponds = factorial()
                .Take(10)
                .Correspond(expectedFactorial);

            Println($"Factorial first 10 values {ConjugateCorrespondVerb(factCorresponds)} to {expectedFactorial.Join(", ")}");
            
            // Fibonacci test

            static Stream<Int> fibonacci() =>
                Stream<Int>(0, 1).Append(() => fibonacci().Zip(fibonacci().Tail).Map(p => p.Item1 + p.Item2));

            var expectedFibonacci = Stream<Int>(0, 1, 1, 2, 3, 5, 8, 13, 21, 34);

            var fibCorresponds = fibonacci()
                .Take(10)
                .Correspond(expectedFibonacci);

            Println($"Fibonacci first 10 values {ConjugateCorrespondVerb(fibCorresponds)} to {expectedFibonacci.Join(", ")}");

            // Test slicing

            var pairs1 = Stream(1, 2, 3, 4, 5, 6)
                .Slice(2, 2)
                .Map(a => a.ToMutableArray())
                .Map(a => (a[0], a[1]));

            var expectedSliced1 = Stream((1, 2), (3, 4), (5, 6));

            var slicedCorresponds1 = pairs1.Correspond(expectedSliced1);

            Println($"Sliced stream {ConjugateCorrespondVerb(slicedCorresponds1)} to {expectedSliced1.Join(", ")}");

            var pairs2 = Stream(1, 2, 3, 4, 5, 6)
                .Slice(2, 1)
                .Map(a => a.ToMutableArray())
                .Map(a => (a[0], a[1]));

            var expectedSliced2 = Stream((1, 2), (2, 3), (3, 4), (4, 5), (5, 6));

            var slicedCorresponds2 = pairs2.Correspond(expectedSliced2);

            Println($"Sliced stream {ConjugateCorrespondVerb(slicedCorresponds2)} to {expectedSliced2.Join(", ")}");
        }

        private static void Println(
            string text,
            ConsoleColor foreColor = ConsoleColor.White,
            ConsoleColor backColor = ConsoleColor.Black
        ) {
            lock (ConsoleAsyncObj)
            {
                var prevForeColor = Console.ForegroundColor;
                var prevBackColor = Console.BackgroundColor;

                Console.ForegroundColor = foreColor;
                Console.BackgroundColor = backColor;

                Console.WriteLine(text);

                Console.ForegroundColor = prevForeColor;
                Console.BackgroundColor = prevBackColor;
            }
        }

        private static readonly object ConsoleAsyncObj = new object();
    }
}
