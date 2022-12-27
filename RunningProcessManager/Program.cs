using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cache;
using Cache.DTOs;
using PerformanceCounterModule.PerformanceCounters.Base;

namespace RunningProcessManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(Function);

            while (true)
            {
                Thread.Sleep(15000);

                var info = ProcessesInfoCache.GetInfo(Guid.Empty)?.PerformanceInfos?.FirstOrDefault(e => e.TypeName == "CPU");

                if (info != null)
                {
                    Console.WriteLine(info.TypeName);

                    foreach(var processInfo in info.ProcessInfos)
                    {
                        Console.WriteLine($"{processInfo.Name}, {processInfo.Value} {info.ProcessInfoUnit}");
                    }
                }
            }
        }

        private static Task Function()
        {
            while (true)
            {
                var i = 1;
                var processes = Process.GetProcesses().Select(e => (i++, e)).ToList();

                var test = PerformanceCounterBase.GetPerformanceCounters();
                var lockObject = new object();
                List<PerformanceInfoElement> list = new List<PerformanceInfoElement>();

                Parallel.ForEach(test, new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount}, e =>
                {
                    var element = e.GetPerformanceInfo(processes);
                    GetList().Add(element);
                });

                List<PerformanceInfoElement> GetList()
                {
                    lock (lockObject)
                    {
                        return list;
                    }
                }

                ProcessesInfoCache.SetInfo(new PerformanceInfoList(Guid.NewGuid(), list));

                Thread.Sleep(10000);
            }
        }
    }
}
