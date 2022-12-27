using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Cache.DTOs;
using PerformanceCounterModule.PerformanceCounters.Base;

namespace PerformanceCounterModule.PerformanceCounters
{
    public class CpuPerformanceCounter : PerformanceCounterBase
    {
        private string _counterName => "% Processor Time";

        /// <summary>
        /// Performance counter type names
        /// </summary>
        protected override string TypeName => "CPU";

        /// <summary>
        /// Unit of return value fo process (like %, Mbps and etc.) 
        /// </summary>
        protected override string ValueUnit => "%";

        /// <summary>
        /// Returns PerformanceInfoElement with ProcessInfo list
        /// </summary>
        /// <param name="processes"></param>
        /// <returns cref="PerformanceInfoElement">Element with ProcessInfo list</returns>
        public override PerformanceInfoElement GetPerformanceInfo(List<(int Id, Process process)> processes)
        {
            var result = new PerformanceInfoElement(TypeName, ValueUnit);

            //Create total performance counter. It is needed to scale values later.
            var totalCounter = new PerformanceCounter("Processor", _counterName, "_Total");
            var processCounters = new List<(int Id, PerformanceCounter counter)>();

            //Сreate counters for all processes
            foreach (var process in processes)
            {
                try
                {
                    processCounters.Add((process.Id, new PerformanceCounter("Process", _counterName, process.process.ProcessName)));
                }
                catch (InvalidOperationException)
                {
                    processCounters.Add((process.Id, null));
                }
            }

            //Get first samples for processes and total Processor
            var sampleDict = new Dictionary<int, CounterSample?>();
            var totalSample = totalCounter.NextSample();

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

            //Calculate value for Processor
            var totalCounterResult = CounterSample.Calculate(totalSample, totalCounter.NextSample());

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
                        counterResult = (counterResult / Environment.ProcessorCount) * (totalCounterResult / 100);
                    }
                }
                catch (InvalidOperationException)
                {
                    
                }
                
                result.ProcessInfos.Add(new ProcessInfo(counter.Id, counter.counter.InstanceName, Math.Round(counterResult, 1).ToString(CultureInfo.InvariantCulture)));
            }

            return result;
        }
    }
}
