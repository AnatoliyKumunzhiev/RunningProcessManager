using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Cache.DTOs;
using PerformanceCounterModule.PerformanceCounters.Base;

namespace PerformanceCounterModule.PerformanceCounters
{
    public sealed class RamPerformanceCounter : PerformanceCounterBase
    {
        /// <summary>
        /// Performance counter type names
        /// </summary>
        protected override string TypeName => "RAM";

        /// <summary>
        /// Unit of return value fo process (like %, Mbps and etc.) 
        /// </summary>
        protected override string ValueUnit => "MB";

        public override PerformanceInfoElement GetPerformanceInfo(List<(int Id, Process process)> processes)
        {
            var result = new PerformanceInfoElement(TypeName, ValueUnit);
            var processCounters = new List<(int Id, string Name, PerformanceCounter counter)>();

            //Create counters for all processes
            foreach (var process in processes)
            {
                try
                {
                    processCounters.Add((process.Id, process.process.ProcessName, new PerformanceCounter("Process", "Working Set - Private", process.process.ProcessName)));
                }
                catch (InvalidOperationException)
                {
                    processCounters.Add((process.Id, process.process.ProcessName, null));
                }
            }

            //Calculate value for all processes and save it in result list
            foreach (var counter in processCounters)
            {
                float counterResult =  0;
                try
                {
                    if (counter.counter != null)
                    {
                        counterResult = counter.counter.NextValue();
                    }
                }
                catch (InvalidOperationException)
                {
                    
                }

                result.ProcessInfos.Add(new ProcessInfo(counter.Id, counter.Name, Math.Round(counterResult / 1000000, 1).ToString(CultureInfo.InvariantCulture)));
            }

            return result;
        }
    }
}
