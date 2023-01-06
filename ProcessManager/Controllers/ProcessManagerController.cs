using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Cache;
using Cache.DTOs;

namespace ProcessManager.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProcessManagerController : ApiController
    {
        private static readonly ConcurrentBag<StreamWriter> Clients;  

        static ProcessManagerController()  
        {  
            Clients = new ConcurrentBag<StreamWriter>();
            ProcessesInfoCache.CpuWarningEvent += OnCpuWarningEvent;
        }

        private static void OnCpuWarningEvent(string message)
        {
            Parallel.ForEach(Clients, new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, async client =>
            {
                using (client)
                {
                    try
                    {
                        await client.WriteLineAsync($"data: {message}\n");
                        await client.FlushAsync();
                    }
                    catch(Exception)
                    {
                        Clients.TryTake(out _);
                    }
                }
            });
        }

        /// <summary>
        /// Get performance info by processes
        /// </summary>
        /// <param name="id">Last requested cache id</param>
        /// <returns cref="PerformanceInfoList">PerformanceInfo</returns>
        public PerformanceInfoList Get(Guid id)
        {
            return ProcessesInfoCache.GetInfo(id);
        }

        /// <summary>
        /// Subscribe on Cpu overload method
        /// </summary>
        /// <param name="request" cref="HttpRequestMessage"></param>
        /// <returns cref="HttpResponseMessage">response</returns>
        [HttpGet]  
        [Route("api/ProcessManager/SubscribeCpuOverload")]
        public HttpResponseMessage SubscribeCpuOverload(HttpRequestMessage request)  
        {  
            var response = request.CreateResponse();  
            response.Content = new PushStreamContent((stream, content, context) =>
            {
                var client = new StreamWriter(stream);  
                Clients.Add(client);  
            }, "text/event-stream");  
            return response;
        }
    }
}
