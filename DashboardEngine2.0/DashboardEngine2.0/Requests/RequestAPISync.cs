using DashboardEngine2._0.AuxClasses;
using DashboardEngine2._0.Model;
using DashboardEngine2._0.StaticClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Requests
{
    public static class RequestAPISync
    {
        static int retryAPI = Convert.ToInt32(ConfigurationSettings.AppSettings["retryAPI"].ToString());
        static int timoutAPI = Convert.ToInt32(ConfigurationSettings.AppSettings["timoutAPI"].ToString());
        static int timeoutGrid = Convert.ToInt32(ConfigurationSettings.AppSettings["timeoutGrid"].ToString());
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Connect connector = new Connect();


        public static string processGetTile(string uri, string path, bool firstTry)
        {


            uri = uri.ToLower();
            JToken item = null;
            JObject jsonObject = null;
            jsonObject = (JObject)MemoryCaches.memCache.Get(uri);
            try
            {

                if (jsonObject != null)
                {
                    if (jsonObject.ToString() == "error")
                        return null;
                }
                else
                {
                    var response = connector.HttpGet(uri, timoutAPI);
                    Logger.Debug("GET: " + uri + " OK");
                    jsonObject = JObject.Parse(response);
                }

                JObject jObject = (JObject)jsonObject;
                if (jObject != null)
                {
                    MemoryCaches.memCache.Set(uri, jObject, MemoryCache.InfiniteAbsoluteExpiration);
                    item = jObject.SelectToken("$." + path);
                    return item.ToString();
                }
                else
                {
                    MemoryCaches.memCache.Set(uri, "error", MemoryCache.InfiniteAbsoluteExpiration);
                    return null;
                }


            }
            catch (Exception ex)
            {
                if (firstTry)
                {
                    Thread.Sleep(retryAPI);
                    return processGetTile(uri, path, false);
                }
                else
                {
                    MemoryCaches.memCache.Set(uri, "error", MemoryCache.InfiniteAbsoluteExpiration);
                    Logger.Warn("GET: " + uri + " " + path);
                    APIResponseTime.calculateResponseCount(uri, ex.GetType().ToString());
                    Logger.Warn(ex.ToString());
                    return null;
                }


            }
        }

        public static  List<string[]> getSyncGrid(string uri, string dashboardID)
        {
            string response;
            try
            {
                
                List<string[]> result = null;

                response = connector.HttpGet(uri, timoutAPI);

                Logger.Debug("GET: " + uri + " ok");

                if (response != null)
                    result = JsonConvert.DeserializeObject<List<string[]>>(response);
                else
                    result = new List<string[]>();
                return result;
            }
            catch (Exception ex)
            {
                APIResponseTime.calculateResponseCount(uri, ex.GetType().ToString());
                Logger.Warn("GET: " + uri);
                Logger.Warn(ex.ToString());
                return null;
            }
        }



    }
}
