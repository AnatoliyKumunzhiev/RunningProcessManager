using System;
using System.Collections.Generic;

namespace Cache.DTOs
{
    public class PerformanceInfoList
    {
        public PerformanceInfoList(Guid id, List<PerformanceInfoElement> performanceInfos, bool usePrevious = false)
        {
            Id = id;
            PerformanceInfos = performanceInfos;
            UsePrevious = usePrevious;
        }

        /// <summary>
        /// Info (cache) Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// True value means, that you already got the same info early. Need to use previous data.
        /// </summary>
        public bool UsePrevious { get; }

        /// <summary>
        /// Processes performance info
        /// </summary>
        public List<PerformanceInfoElement> PerformanceInfos { get; }
    }
}
