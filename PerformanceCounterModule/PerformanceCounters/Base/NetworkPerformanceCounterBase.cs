using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Cache.DTOs;

namespace PerformanceCounterModule.PerformanceCounters.Base
{
    public abstract class NetworkPerformanceCounterBase : PerformanceCounterBase
    {
        /// <summary>
        /// Unit of return value fo process (like %, Mbps and etc.) 
        /// </summary>
        protected sealed override string ValueUnit => "Mbps";

        /// <summary>
        /// Network counter name for process
        /// </summary>
        protected abstract string ProcessCounterName { get; }

        /// <summary>
        /// Returns PerformanceInfoElement with ProcessInfo list
        /// </summary>
        /// <param name="processes"></param>
        /// <returns cref="PerformanceInfoElement">Element with ProcessInfo list</returns>
        public sealed override PerformanceInfoElement GetPerformanceInfo(List<(int Id, Process process)> processes)
        {
            var result = new PerformanceInfoElement(TypeName, ValueUnit);
            var processCounters = new List<(int Id, string Name, PerformanceCounter counter)>();

            //Сreate counters for all processes
            foreach (var process in processes)
            {
                try
                {
                    processCounters.Add((process.Id, process.process.ProcessName, new PerformanceCounter("Process", ProcessCounterName, process.process.ProcessName)));
                }
                catch (InvalidOperationException)
                {
                    processCounters.Add((process.Id, process.process.ProcessName, null));
                }
            }

            //Get first samples for processes
            var sampleDict = new Dictionary<int, CounterSample?>();
            foreach (var counter in processCounters)
            {
                try
                {
                    sampleDict.Add(counter.Id, counter.counter?.NextSample());
                }
                catch (InvalidOperationException)
                {
                    sampleDict.Add(counter.Id, null);
                }
            }

            Thread.Sleep(1000);

            //Calculate value for all processes and save it in result list
            foreach (var counter in processCounters)
            {
                var prevSample = sampleDict[counter.Id];
                float counterResult =  0;
                try
                {
                    if (counter.counter != null)
                    {
                        counterResult = prevSample == null ? 0 : CounterSample.Calculate(prevSample.Value, counter.counter.NextSample());
                    }
                }
                catch (InvalidOperationException)
                {
                    
                }

                result.ProcessInfos.Add(new ProcessInfo(counter.Id, counter.Name, Math.Round((counterResult / 1000000) * 8, 1).ToString(CultureInfo.InvariantCulture)));
            }

            return result;
        }
    }
}
