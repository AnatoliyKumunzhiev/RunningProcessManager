using PerformanceCounterModule.PerformanceCounters.Base;

namespace PerformanceCounterModule.PerformanceCounters
{
    public sealed class NetworkSentPerformanceCounter : NetworkPerformanceCounterBase
    {
        /// <summary>
        /// Performance counter type names
        /// </summary>
        protected override string TypeName => "Network sent";

        /// <summary>
        /// Network counter name for process
        /// </summary>
        protected override string ProcessCounterName => "IO Write Bytes/sec";
    }
}
