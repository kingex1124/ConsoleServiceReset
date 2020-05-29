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
            Program.RestartWebSiteService("WebTest", 2000);

            //Program.RestartService("StockGateway", 15000);
            Program.RestartService("ATest", 15000);
            Console.WriteLine("end");
            Console.ReadLine();
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
    
                var site = server.Sites.FirstOrDefault(s => s.Name == webSiteName);

                // 取得站台目前狀態
                Console.WriteLine(site.State);
                if (site != null)
                {                 
                    //stop the site
                    site.Stop();

                    // 取得停止站台後的狀態
                    Console.WriteLine(site.State);

                    while (site.State.ToString() == "Stopped")
                    {
                        //等待一段時間
                        Thread.Sleep(timeout);

                        //start the site
                        site.Start();
                    }
                    // 取得啟動站台後的狀態
                    Console.WriteLine(site.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                //取得服務目前狀態
                Console.WriteLine(service.Status);

                //啟動中就關閉重啟，如果是關閉中，就直接啟動
                if (service.Status.ToString() == "Started")
                {
                    service.Stop();
                    //執行停止
                    // 設置timeout表示等候關閉 或是啟動的時間，不設定表示無止境等待
                    //service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    service.WaitForStatus(ServiceControllerStatus.Stopped);

                    //取得服務停止後的狀態
                    Console.WriteLine("停止後:" + service.Status);
                }
                // retry
                while (service.Status.ToString() == "Stopped")
                {
                    service.Start();
                    //執行啟動
                    //service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }

                //取得服務重啟後的狀態
                Console.WriteLine("重啟後:" + service.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }   
        }
    }
}
