using DashboardEngine2._0.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DashboardEngine2._0.StaticClasses
{
    public class dashMemory
    {
        public Dashboard dashboard { get; set; }
        public string dashboardId { get; set; }
        public bool active { get; set; }
    }
    public static class MemoryCaches
    {
        public static List<dashMemory> memDash = new List<dashMemory>();
        public static Dictionary<string, string> memThings = new Dictionary<string, string>();
        public static Dictionary<string, string> memMessages = new Dictionary<string, string>();
        public static MemoryCache memCache = MemoryCache.Default;

        public static void clearCache()
        {
            lock (memCache)
            {
                List<string> cacheKeys = memCache.Select(kvp => kvp.Key).ToList();
                foreach (string cacheKey in cacheKeys)
                {
                    memCache.Remove(cacheKey);
                }
            }
        }

    }
}
