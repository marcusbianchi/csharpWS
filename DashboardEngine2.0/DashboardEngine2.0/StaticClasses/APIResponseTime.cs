using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardEngine2._0.StaticClasses
{
    /// <summary>
    /// Claas which calculates the avarage response time for each API
    /// and also counts the number of each type of error
    /// </summary>
    public static class APIResponseTime
    {
        public static Dictionary<string, TimeSpan> responseTime = new Dictionary<string, TimeSpan>();
        public static Dictionary<string, Dictionary<string, int>> errorCount = new Dictionary<string, Dictionary<string, int>>();
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Updates the current avarage response time for a certain API
        /// </summary>
        /// <param name="api">Name of the API</param>
        /// <param name="currentTime">Time span of the last call</param>
        public static void calculateResponseTime(string api, TimeSpan currentTime)
        {
            api = api.ToLower();
            lock (responseTime)
            {
                if (!responseTime.ContainsKey(api))
                {
                    responseTime.Add(api, currentTime);
                }
                else
                {
                    var newCurrent = (responseTime[api] + currentTime).TotalMilliseconds / 2;
                    responseTime[api] = TimeSpan.FromMilliseconds(newCurrent);
                }
            }
        }

        /// <summary>
        /// Write according to log the response time for each API
        /// </summary>
        public static void writeResponseTime()
        {
            Logger.Info("___________________________________________________");
            lock (responseTime)
            {
                foreach (var item in responseTime)
                {
                    Logger.Info(item.Key + " Tempo Médio: " + item.Value);
                }
                responseTime = new Dictionary<string, TimeSpan>();
            }
            Logger.Info("___________________________________________________");
        }

        /// <summary>
        /// Write according to log the error count for each API
        /// </summary>
        public static void writeErrorCount()
        {
            Logger.Info("___________________________________________________");
            lock (errorCount)
            {
                foreach (var item in errorCount)
                {
                    Logger.Info(item.Key);
                    foreach (var Error in item.Value)
                    {
                        Logger.Info("====> Erro: " + Error.Key + " Contagem: " + Error.Value);
                    }
                }
                errorCount = new Dictionary<string, Dictionary<string, int>>();
            }

            Logger.Info("___________________________________________________");
        }

        /// <summary>
        // Updates the current error count  for a certain API e separete by error name
        /// </summary>
        /// <param name="api">Name of the API</param>
        /// <param name="error">Error type</param>
        public static void calculateResponseCount(string api, string error)
        {
            var url = new Uri(api.ToLower());
            api = url.AbsolutePath;
            lock (errorCount)
            {
                if (!errorCount.ContainsKey(api))
                {
                    errorCount.Add(api, new Dictionary<string, int>());
                    errorCount[api].Add(error, 1);
                }
                else
                {
                    if (!errorCount[api].ContainsKey(error))
                    {
                        errorCount[api].Add(error, 1);
                    }
                    else
                    {
                        errorCount[api][error] += 1;
                    }
                }
            }
        }
    }
}
