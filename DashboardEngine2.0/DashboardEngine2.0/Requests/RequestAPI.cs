using DashboardEngine2._0.Classes;
using DashboardEngine2._0.Model;
using DashboardEngine2._0.StaticClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Requests
{
    public static class RequestAPI
    {
        static int timoutAPI = Convert.ToInt32(ConfigurationSettings.AppSettings["timoutAPI"].ToString());
        static int timeoutMesssages = Convert.ToInt32(ConfigurationSettings.AppSettings["timeoutMesssages"].ToString());
        static int timeoutGrid = Convert.ToInt32(ConfigurationSettings.AppSettings["timeoutGrid"].ToString());
        static int retryAPI = Convert.ToInt32(ConfigurationSettings.AppSettings["retryAPI"].ToString());

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static async Task<Dashboard> getAsyncDashboard(string uri)
        {
            try
            {
                string respString = null;
                Dashboard result = null;

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        respString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<Dashboard>(respString);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("GET: " + uri);
                Logger.Error(ex.ToString());
                throw ex;
            }
        }

        public static async Task<DateTime?> getAsyncLastAccessCache(string uri)
        {
            try
            {
                string respString = null;
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        respString = await response.Content.ReadAsStringAsync();                        
                    }
                }
                if (respString == null)
                    return null;
                return new DateTime(Convert.ToInt64(respString));
            }
            catch (Exception ex)
            {
                Logger.Error("GET: " + uri);
                Logger.Error(ex.ToString());
                throw ex;
            }
        }
        public static async void PostAsyncCache(string uri, string contentJson, string dashboardConfigId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(uri, content);
                    Logger.Debug("POST: " + uri + " " + result.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Error("POST: " + uri + dashboardConfigId);
                Logger.Error(ex.ToString());
            }
        }
        public static async void DelAsyncCache(string uri)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = await client.DeleteAsync(uri);
                    Logger.Debug("DEL: " + uri + " " + result.StatusCode.ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.Error("DEL: " + uri);
                Logger.Error(ex.ToString());
            }
        }
        public static async Task<List<Dashboard>> getAsyncAllDashboards(string uri)
        {
            try
            {
                string respString = null;
                List<Dashboard> result = null;
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());
                    respString = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<Dashboard>>(respString);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("GET: " + uri);
                Logger.Warn(ex.ToString());
                throw ex;
            }
        }
        public static async Task<string> processGetTile(string uri, string path, bool firstTry)
        {
            

            uri = uri.ToLower();
            string respString = null;
            JToken item = null;
            JObject jsonObject = null;
            jsonObject = (JObject)MemoryCaches.memCache.Get(uri);
            HttpResponseMessage response = null;
            var cts = new CancellationTokenSource();
            try
            {

                if (jsonObject != null)
                {
                    if (jsonObject.ToString() == "error")
                        return null;
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        //client.Timeout = TimeSpan.FromMilliseconds(timoutAPI);
                        response = await client.GetAsync(uri);
                        Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            respString = await response.Content.ReadAsStringAsync();
                            jsonObject = JObject.Parse(respString);
                        }
                        else
                        {
                            respString = await response.Content.ReadAsStringAsync();
                            APIResponseTime.calculateResponseCount(uri, response.StatusCode.ToString());
                        }
                    }

                }

                JObject jObject = (JObject)jsonObject;
                if (jObject != null)
                {
                    MemoryCaches.memCache.Set(uri, jObject,MemoryCache.InfiniteAbsoluteExpiration);
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
                    await Task.Delay(retryAPI);
                    return await processGetTile(uri, path, false);
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
        public static async Task<List<string[]>> getAsyncGrid(string uri, string dashboardID)
        {
            try
            {

                string respString = null;
                List<string[]> result = null;
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeoutGrid);
                    var response = await client.GetAsync(uri);
                    Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        respString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<List<string[]>>(respString);
                    }
                    else
                    {
                        APIResponseTime.calculateResponseCount(uri, response.StatusCode.ToString());
                    }
                }
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

        public static async Task<Dictionary<string, string>> getAsyncDisplayIPs(string uri)
        {
            try
            {
                string respString = null;
                List<JObject> result = null;
                Dictionary<string, string> listResult = new Dictionary<string, string>();
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());
                    respString = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<JObject>>(respString);
                }
                JToken ip = null;
                JToken Message = null;
                foreach (var obj in result)
                {
                    ip = obj.SelectToken("$." + "ipAdress");
                    Message = obj.SelectToken("$." + "message");
                    if (!listResult.ContainsKey(ip.ToString()))
                        listResult.Add(ip.ToString(), Message==null?"" : Message.ToString());
                }

                return listResult;
            }
            catch (Exception ex)
            {
                Logger.Warn("GET: " + uri);
                Logger.Warn(ex.ToString());
                return null;
            }
        }

        public static async Task<Dictionary<string, string>> getAsyncThings(string uri)
        {
            try
            {
                string respString = null;
                List<JObject> result = null;
                Dictionary<string, string> dictResult = new Dictionary<string, string>();
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        respString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<List<JObject>>(respString);
                    }
                    else
                    {
                        APIResponseTime.calculateResponseCount(uri, response.StatusCode.ToString());

                        return null;
                    }
                }
                JToken item = null;
                JToken value = null;
                foreach (var obj in result)
                {
                    item = obj.SelectToken("$." + "thingId");
                    value = obj.SelectToken("$." + "code");
                    dictResult.Add(item.ToString(), value.ToString());
                }

                return dictResult;
            }
            catch (Exception ex)
            {
                APIResponseTime.calculateResponseCount(uri, ex.GetType().ToString());

                Logger.Warn("GET: " + uri);
                Logger.Warn(ex.ToString());
                return null;
            }
        }

        public static async Task<Dictionary<string, string>> getAsyncMessages(string uri, FilterStatus filter, string dashboardID)
        {
            try
            {

                string respString = null;
                List<JObject> result = null;
                Dictionary<string, string> messages = new Dictionary<string, string>();
                string URL = uri + "?filter=" + JsonConvert.SerializeObject(filter);
                URL = URL.Replace(@"\", string.Empty);
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeoutMesssages);
                    var response = await client.GetAsync(URL);
                    Logger.Debug("GET: " + uri + " " + response.StatusCode.ToString());
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        respString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<List<JObject>>(respString);
                    }
                    else
                    {
                        APIResponseTime.calculateResponseCount(uri, response.StatusCode.ToString());
                        return null;
                    }

                }
                JToken thing = null;
                JToken item = null;
                foreach (var obj in result)
                {
                    thing = obj.SelectToken("$." + "thingId");
                    if (!messages.ContainsKey(thing.ToString()))
                    {
                        item = obj.SelectToken("$." + "status");
                        messages.Add(thing.ToString(), item.ToString());

                    }
                }
                return messages;
            }
            catch (Exception ex)
            {
                APIResponseTime.calculateResponseCount(uri, ex.GetType().ToString());
                Logger.Warn(uri);
                Logger.Warn(JsonConvert.SerializeObject(filter));
                Logger.Warn(ex.ToString());
                return null;
            }
        }

    }
}
