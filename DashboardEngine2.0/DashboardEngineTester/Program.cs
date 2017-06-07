using DashboardEngine2._0;
using DashboardEngine2._0.StaticClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DashboardEngineTester
{
    class Program
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {



            Console.Title = "Async";
            var x = Task.Run(() => executeStart());
            Console.ReadLine();

        }

        public static async Task executeStart()
        {
            try
            {
                int refreshTime = Convert.ToInt32(ConfigurationSettings.AppSettings["DashCacheRefreshRateSec"].ToString());
                string dashBoards = ConfigurationSettings.AppSettings["dashBoards"].ToString();
                string thingsURL = ConfigurationSettings.AppSettings["thingsURL"].ToString();
                string displayURL = ConfigurationSettings.AppSettings["displayURL"].ToString();
                string LastAccessURL = ConfigurationSettings.AppSettings["LastAccessURL"].ToString();
                double MaxTimeCache = Convert.ToDouble(ConfigurationSettings.AppSettings["MaxTimeCacheMin"].ToString());
                string cacheURlMain = ConfigurationSettings.AppSettings["cacheURl"].ToString();
                bool PararellProcessDashboard = Convert.ToBoolean(ConfigurationSettings.AppSettings["PararellProcessDashboard"].ToString());
                string server = ConfigurationSettings.AppSettings["server"].ToString();
                string cacheURl = ConfigurationSettings.AppSettings["cacheURl"].ToString();
                string errorColor = ConfigurationSettings.AppSettings["errorColor"].ToString();
                bool PararellProcessTiles = Convert.ToBoolean(ConfigurationSettings.AppSettings["PararellProcessTiles"].ToString());
                bool PararellProcessGrid = Convert.ToBoolean(ConfigurationSettings.AppSettings["PararellProcessGrid"].ToString());
                
                mainProcess mProc = new mainProcess(dashBoards, thingsURL, displayURL, LastAccessURL, cacheURlMain, PararellProcessDashboard, MaxTimeCache, server, cacheURl, errorColor, PararellProcessTiles, PararellProcessGrid);
                await mProc.ThingsCache();
                await mProc.DisplayCache();
                await mProc.dashBoardCache();
                DateTime lastUpdateCaches = DateTime.Now;
                while (true)
                {
                    var starTime = DateTime.Now;
                    if ((DateTime.Now - lastUpdateCaches) >= TimeSpan.FromSeconds(refreshTime))
                    {
                        lastUpdateCaches = DateTime.Now;
                        Thread updateCacheThread = new Thread(updateCache);
                        updateCacheThread.Start();
                    }
                    var start = DateTime.Now;
                    await mProc.ProcessAllDashboard();

                    Logger.Info("=======================================================");
                    Logger.Info("++++++++++++++++++API " + (DateTime.Now - start) + "++++++++++++++++++");
                    Logger.Info("=======================================================");
                    APIResponseTime.writeResponseTime();
                    APIResponseTime.writeErrorCount();

                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public static async void updateCache()
        {
            try
            {
                var startCache = DateTime.Now;
                string dashBoards = ConfigurationSettings.AppSettings["dashBoards"].ToString();
                string thingsURL = ConfigurationSettings.AppSettings["thingsURL"].ToString();
                string displayURL = ConfigurationSettings.AppSettings["displayURL"].ToString();
                string LastAccessURL = ConfigurationSettings.AppSettings["LastAccessURL"].ToString();
                double MaxTimeCache = Convert.ToDouble(ConfigurationSettings.AppSettings["MaxTimeCacheMin"].ToString());
                string cacheURlMain = ConfigurationSettings.AppSettings["cacheURl"].ToString();
                bool PararellProcessDashboard = Convert.ToBoolean(ConfigurationSettings.AppSettings["PararellProcessDashboard"].ToString());
                string server = ConfigurationSettings.AppSettings["server"].ToString();
                string cacheURl = ConfigurationSettings.AppSettings["cacheURl"].ToString();
                string errorColor = ConfigurationSettings.AppSettings["errorColor"].ToString();
                bool PararellProcessTiles = Convert.ToBoolean(ConfigurationSettings.AppSettings["PararellProcessTiles"].ToString());
                bool PararellProcessGrid = Convert.ToBoolean(ConfigurationSettings.AppSettings["PararellProcessGrid"].ToString());
                mainProcess mProc = new mainProcess(dashBoards, thingsURL, displayURL, LastAccessURL, cacheURlMain, PararellProcessDashboard, MaxTimeCache, server, cacheURl, errorColor, PararellProcessTiles, PararellProcessGrid);
                await mProc.ThingsCache();
                await mProc.DisplayCache();
                await mProc.dashBoardCache();
                Logger.Info("=======================================================");
                Logger.Info("------------------CACHE " + (DateTime.Now - startCache) + "------------------");
                Logger.Info("=======================================================");


            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
    }
}
