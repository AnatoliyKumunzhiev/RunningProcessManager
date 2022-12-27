using PerformanceCounterModule.PerformanceCounters.Base;

namespace PerformanceCounterModule.PerformanceCounters
{
    public sealed class NetworkReceivedPerformanceCounter : NetworkPerformanceCounterBase
    {
        /// <summary>
        /// Performance counter type names
        /// </summary>
        protected override string TypeName => "Network received";

        /// <summary>
        /// Network counter name for process
        /// </summary>
        protected override string ProcessCounterName => "IO Read Bytes/sec";
    }
}
