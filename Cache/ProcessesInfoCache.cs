using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cache.DTOs;

namespace Cache
{
    /// <summary>
    /// Static cache class for performance info storage
    /// </summary>
    public static class ProcessesInfoCache
    {
        private static readonly object Lock;

        private static float CpuOverloadLevel => 10;

        static ProcessesInfoCache()
        {
            Lock = new object();
        }

        /// <summary>
        /// Performance info list with info about processes
        /// </summary>
        private static PerformanceInfoList PerformanceInfoList { get; set; }

        /// <summary>
        /// Set cache
        /// </summary>
        /// <param name="performanceInfoList" cref="DTOs.PerformanceInfoList">New cache object with new Guid Id and info about processes</param>
        public static void SetInfo(PerformanceInfoList performanceInfoList)
        {
            lock (Lock)
            {
                PerformanceInfoList = performanceInfoList;
            }

            Task.Run(() =>
            {
                var totalCpuLoad = GetInfo()?.PerformanceInfos.FirstOrDefault(e => string.Equals(e.TypeName, "CPU"))?.ProcessInfos.Sum(e => float.Parse(e.Value, CultureInfo.InvariantCulture));
                if (totalCpuLoad > CpuOverloadLevel)
                {
                    var message = $"CPU is overloaded more then 10% ({totalCpuLoad}%, {DateTime.Now:dd.MM.yyyy HH:mm:ss})";
                    RaiseCpuWarningEvent(message);
                }
            });
        }

        /// <summary>
        /// Get cache. If cache Id equals Id from params, return empty info with UsePrevious flag
        /// </summary>
        /// <param name="id">Previous cache Id</param>
        /// <returns cref="DTOs.PerformanceInfoList">Performance info</returns>
        public static PerformanceInfoList GetInfo(Guid id)
        {
            var info = GetInfo();

            if (info == null || info.Id == id)
                return new PerformanceInfoList(id, null, true);
            
            return info;
        }

        /// <summary>
        /// Get cache private method with lock
        /// </summary>
        /// <returns cref="DTOs.PerformanceInfoList">Performance info</returns>
        private static PerformanceInfoList GetInfo()
        {
            lock (Lock)
            {
                return PerformanceInfoList;
            }
        }

        /// <summary>
        /// Cpu overload delegate
        /// </summary>
        public delegate void CpuWarningEventHandler(string message);

        /// <summary>
        /// Cpu overload event
        /// </summary>
        public static event CpuWarningEventHandler CpuWarningEvent;

        /// <summary>
        /// Cpu overload raise method
        /// </summary>
        /// /// <param name="message">Message for clients</param>
        private static void RaiseCpuWarningEvent(string message)
        {
            CpuWarningEvent?.Invoke(message);
        }
    }
}
