using System.Collections.Generic;

namespace Cache.DTOs
{
    public class PerformanceInfoElement
    {
        public PerformanceInfoElement(string typeName, string processInfoUnit)
        {
            TypeName = typeName;
            ProcessInfoUnit = processInfoUnit;
        }

        private List<ProcessInfo> _processInfos;

        /// <summary>
        /// Performance type name
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Performance value unit
        /// </summary>
        public string ProcessInfoUnit { get; }

        /// <summary>
        /// Process performance info list
        /// </summary>
        public List<ProcessInfo> ProcessInfos => _processInfos ??= new List<ProcessInfo>();
    }
}
