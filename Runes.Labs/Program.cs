using Runes.Async;
using Runes.Async.Jobs;
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
            //
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
