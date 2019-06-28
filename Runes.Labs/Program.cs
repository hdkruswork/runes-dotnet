using Runes.Async;
using Runes.Async.Jobs;
using Runes.Security.Cryptography;
using Runes.Text;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Runes.Labs
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadAssemblies();

            var hexDecoder = HexDecoder.Object;
            var sha256 = Sha256Algorithm.Object;

            Console.WriteLine("Testing execution pool");

            var firstTaskName = "1st Task";
            var firstTaskId = hexDecoder.Decode(sha256.Compute(firstTaskName, Encoding.UTF8));

            var secondTaskName = "2nd Task";
            var secondTaskId = hexDecoder.Decode(sha256.Compute(secondTaskName, Encoding.UTF8));

            var executorPool = AppDomain
                .CurrentDomain
                .CreateInstance("Runes.Async", "Runes.Async.ExecutersPool")
                .Unwrap();

            //Console.WriteLine($"Pool size: {executorPool.PoolSize}");

            //var consoleSyncObj = new object();
            //void UpdateSatus(string id, IJobStatus status)
            //{
            //    lock (consoleSyncObj)
            //    {
            //        Console.WriteLine($"Job: {id} : {status}");
            //    }
            //}

            //executorPool.Execute(
            //    firstTaskId,
            //    firstTaskName,
            //    () => {
            //        System.Threading.Thread.Sleep(5000);
            //        return 120;
            //    },
            //    status => UpdateSatus(firstTaskName, status)
            //);

            //executorPool.Execute(
            //    secondTaskId,
            //    secondTaskName,
            //    () => {
            //        System.Threading.Thread.Sleep(4000);
            //        return 480;
            //    },
            //    status => UpdateSatus(secondTaskName, status)
            //);

            //executorPool.WaitForAll();
        }

        private static void LoadAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => {
                loadedAssemblies.Add(Assembly.LoadFrom(path));
            });
        }
    }
}
