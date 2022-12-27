using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cache.DTOs;

namespace PerformanceCounterModule.PerformanceCounters.Base
{
    public abstract class PerformanceCounterBase
    {
        /// <summary>
        /// Performance counter type names
        /// </summary>
        protected abstract string TypeName { get; }

        /// <summary>
        /// Unit of return value fo process (like %, Mbps and etc.) 
        /// </summary>
        protected abstract string ValueUnit { get; }

        /// <summary>
        /// Returns PerformanceInfoElement with ProcessInfo list
        /// </summary>
        /// <param name="processes"></param>
        /// <returns cref="PerformanceInfoElement">Element with ProcessInfo list</returns>
        public abstract PerformanceInfoElement GetPerformanceInfo(List<(int Id, Process process)> processes);

        /// <summary>
        /// Returns array of base class instances
        /// </summary>
        /// <param></param>
        /// <returns>Instance array of this base class</returns>
        public static PerformanceCounterBase[] GetPerformanceCounters()
        {
            List<PerformanceCounterBase> counterList = new List<PerformanceCounterBase>();
            var abstractType = typeof(PerformanceCounterBase);
            foreach (var type in abstractType.Assembly.GetTypes().Where(t => !t.IsAbstract && abstractType.IsAssignableFrom(t)))
            {
                var constructor = type.GetConstructor(new Type[0]);
                if (constructor != null)
                {
                    counterList.Add((PerformanceCounterBase)constructor.Invoke(new object[0]));
                }
            }

            return counterList.ToArray();
        }
    }
}
