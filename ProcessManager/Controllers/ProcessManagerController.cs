using System;
using System.Web.Http;
using System.Web.Http.Cors;
using Cache;
using Cache.DTOs;

namespace ProcessManager.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProcessManagerController : ApiController
    {
        /// <summary>
        /// Get performance info by processes
        /// </summary>
        /// <param name="id">Last requested cache id</param>
        /// <returns cref="PerformanceInfoList">PerformanceInfo</returns>
        public PerformanceInfoList Get(Guid id)
        {
            return ProcessesInfoCache.GetInfo(id);
        }
    }
}
