using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Cache;
using Cache.DTOs;
using PerformanceCounterModule.PerformanceCounters.Base;

namespace ProcessManager
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Run refresh performance info task
            Task.Run(() =>
            {
                while (true)
                {
                    var i = 1;
                    var processes = Process.GetProcesses().Select(e => (i++, e)).ToList();

                    var counters = PerformanceCounterBase.GetPerformanceCounters();
                    var lockObject = new object();
                    List<PerformanceInfoElement> list = new List<PerformanceInfoElement>();

                    Parallel.ForEach(counters, new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, counter =>
                    {
                        var element = counter.GetPerformanceInfo(processes);
                        SetList(element);
                    });

                    ProcessesInfoCache.SetInfo(new PerformanceInfoList(Guid.NewGuid(), list));

                    Thread.Sleep(10000);

                    void SetList(PerformanceInfoElement element)
                    {
                        lock (lockObject)
                        {
                            list.Add(element);
                        }
                    }
                }
            });
        }
    }
}
