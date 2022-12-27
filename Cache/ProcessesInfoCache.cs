using System;
using Cache.DTOs;

namespace Cache
{
    /// <summary>
    /// Static cache class for performance info storage
    /// </summary>
    public static class ProcessesInfoCache
    {
        private static readonly object Lock;

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
            {
                return new PerformanceInfoList(id, null, true);
            }

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
    }
}
