using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleServiceReset
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.RestartWebSiteService("WebTest", 10);
            //Program.RestartService("ATest", 10000);
        }

        /// <summary>
        /// reset IIS web Site service
        /// </summary>
        /// <param name="webSiteName"></param>
        /// <param name="timeoutMilliseconds"></param>
        public static void RestartWebSiteService(string webSiteName,int timeoutMilliseconds)
        {
            var server = new ServerManager();

            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                var site = server.Sites.FirstOrDefault(s => s.Name == "webSiteName");
                if (site != null)
                {
                    //stop the site
                    site.Stop();

                    Thread.Sleep(timeout);

                    //start the site
                    site.Start();
                }
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Reset Windows Service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="timeoutMilliseconds"></param>
        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                // ...
            }   
        }
    }
}
