using Runes.Async;
using Runes.Async.Jobs;
using Runes.Collections;
using Runes.Math;
using Runes.Security.Cryptography;
using Runes.Text;
using System;
using System.Text;

using static Runes.Predef;

namespace Runes.Labs
{
    class Program
    {
        static void Main()
        {
            //LoadAssemblies();

            TestStreams();
        }

        private static void TestExecutionPool()
        {
            //var executorPool = AppDomain
            //    .CurrentDomain
            //    .CreateInstance("Runes.Async", "Runes.Async.ExecutersPool")
            //    .Unwrap();

            var hexDecoder = HexDecoder.Object;
            var sha256 = Sha256Algorithm.Object;

            Console.WriteLine("Testing execution pool");

            var firstTaskName = "1st Task";
            var firstTaskId = hexDecoder.Decode(sha256.Compute(firstTaskName, Encoding.UTF8));

            var secondTaskName = "2nd Task";
            var secondTaskId = hexDecoder.Decode(sha256.Compute(secondTaskName, Encoding.UTF8));

            Console.WriteLine($"Curr dir: {AppDomain.CurrentDomain.BaseDirectory}");

            var executorPool = new ExecutersPool();

            Console.WriteLine($"Pool size: {executorPool.PoolSize}");

            var consoleSyncObj = new object();
            Action<string, IJobStatus> updateStatus = (id, status) =>
            {
                lock (consoleSyncObj)
                {
                    switch (status)
                    {
                        case DoneWithResult done when done.Result is Success<object> success:
                            Console.WriteLine($"Job: {id} - Done with result: {success.Result} in {done.TimeRange.Interval.InMilliSeconds.WholePart} ms");
                            break;

                        case DoneWithResult done when done.Result is Failure<object> fail:
                            Console.WriteLine($"Job: {id} - Failed: {fail.Exception} in {done.TimeRange.Interval.InMilliSeconds.WholePart} ms");
                            break;

                        case RunningWithProgress progress:
                            Console.WriteLine($"Job: {id} - Running: {progress.Progress}% {progress.ETA.ToOption().Map(eta => $"ETA {eta}s").GetOrElse("")}");
                            break;

                        default:
                            Console.WriteLine($"Job: {id} : {status}");
                            break;
                    }
                }
            };

            executorPool.Execute(
                firstTaskId,
                firstTaskName,
                update => {
                    System.Threading.Thread.Sleep(500);
                    update(Known(1000L), 33);
                    System.Threading.Thread.Sleep(500);
                    update(Known(500L), 66);
                    System.Threading.Thread.Sleep(500);

                    return 120;
                },
                updateStatus.Curry(firstTaskName)
            );

            executorPool.Execute(
                secondTaskId,
                secondTaskName,
                update => {
                    System.Threading.Thread.Sleep(750);
                    return 480;
                },
                updateStatus.Curry(secondTaskName)
            );

            executorPool.WaitForAll();
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

            Console.WriteLine($"Factorial first 10 values {ConjugateCorrespondVerb(factCorresponds)} to {expectedFactorial.Join(", ")}");
            
            // Fibonacci test

            static Stream<Int> fibonacci() =>
                Stream<Int>(0, 1).Append(() => fibonacci().Zip(fibonacci().Tail).Map(p => p.Item1 + p.Item2));

            var expectedFibonacci = Stream<Int>(0, 1, 1, 2, 3, 5, 8, 13, 21, 34);

            var fibCorresponds = fibonacci()
                .Take(10)
                .Correspond(expectedFibonacci);

            Console.WriteLine($"Fibonacci first 10 values {ConjugateCorrespondVerb(fibCorresponds)} to {expectedFibonacci.Join(", ")}");

            // Test slicing

            var pairs1 = Stream(1, 2, 3, 4, 5, 6)
                .Slice(2, 2)
                .Map(a => a.ToMutableArray())
                .Map(a => (a[0], a[1]));

            var expectedSliced1 = Stream((1, 2), (3, 4), (5, 6));

            var slicedCorresponds1 = pairs1.Correspond(expectedSliced1);

            Console.WriteLine($"Sliced stream {ConjugateCorrespondVerb(slicedCorresponds1)} to {expectedSliced1.Join(", ")}");

            var pairs2 = Stream(1, 2, 3, 4, 5, 6)
                .Slice(2, 1)
                .Map(a => a.ToMutableArray())
                .Map(a => (a[0], a[1]));

            var expectedSliced2 = Stream((1, 2), (2, 3), (3, 4), (4, 5), (5, 6));

            var slicedCorresponds2 = pairs2.Correspond(expectedSliced2);

            Console.WriteLine($"Sliced stream {ConjugateCorrespondVerb(slicedCorresponds2)} to {expectedSliced2.Join(", ")}");
        }

        //private static void LoadAssemblies()
        //{
        //    var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        //    var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

        //    var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        //    var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
        //    toLoad.ForEach(path => {
        //        loadedAssemblies.Add(Assembly.LoadFrom(path));
        //    });
        //}
    }
}
