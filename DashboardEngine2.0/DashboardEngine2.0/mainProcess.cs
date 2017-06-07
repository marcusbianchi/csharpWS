using DashboardEngine2._0.Classes;
using DashboardEngine2._0.StaticClasses;
using DashboardEngine2._0.Model;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DashboardEngine2._0.Requests;

namespace DashboardEngine2._0
{

    public class mainProcess
    {
        private  string dashBoards {get;set; }
        private  string thingsURL { get; set; }
        private  string displayURL { get; set; }
        private  string LastAccessURL { get; set; }
        private  string cacheURlMain { get; set; }
        private  bool PararellProcessDashboard { get; set; }
        private  double MaxTimeCache { get; set; }
        private string server { get; set; }
        private string cacheURl { get; set; }
        private string errorColor { get; set; }
        private bool PararellProcessTiles { get; set; }
        private bool PararellProcessGrid { get; set; }
        public mainProcess(string dashBoards,string thingsURL, string displayURL, string LastAccessURL, string cacheURlMain, bool PararellProcessDashboard, double MaxTimeCache,string server, string cacheURl, string errorColor, bool PararellProcessTiles, bool PararellProcessGrid)
        {
            this.dashBoards = dashBoards;
            this.thingsURL = thingsURL;
            this.displayURL = displayURL;
            this.LastAccessURL = LastAccessURL;
            this.cacheURlMain = cacheURlMain;
            this.PararellProcessDashboard = PararellProcessDashboard;
            this.MaxTimeCache = MaxTimeCache;
            this.server = server;
            this.cacheURl = cacheURl;
            this.errorColor = errorColor;
            this.PararellProcessTiles = PararellProcessTiles;
            this.PararellProcessGrid = PararellProcessGrid;
        }


        public static DateTime startDate;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public async Task ProcessAllDashboard()
        {
            ProcessDashboard pDash = new ProcessDashboard(server, cacheURl, errorColor, PararellProcessTiles, PararellProcessGrid);
            try
            {
                List<Task> tasks = new List<Task>();
                List<dashMemory> localDict;
                lock (MemoryCaches.memDash)
                    localDict = new List<dashMemory>(MemoryCaches.memDash);
                foreach (var item in localDict.Where(x => x.active == true))
                {
                    var Dashboard = (Dashboard)item.dashboard;
                    if (PararellProcessDashboard)
                        tasks.Add(pDash.processDashboard(Dashboard));
                    if (!PararellProcessDashboard)
                        await pDash.processDashboard(Dashboard);
                }
                if (PararellProcessDashboard)
                    await Task.WhenAll(tasks.ToArray());
                MemoryCaches.clearCache();
                await DisableUnusedDash();

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public async Task dashBoardCache()
        {
            try
            {
                var dashboarList = await RequestAPI.getAsyncAllDashboards(dashBoards);
                foreach (var dashboardLazy in dashboarList)
                {
                    if (dashboardLazy.disableShow != null)
                        dashboardLazy.disableShow = dashboardLazy.disableShow.ToLower();
                    if (dashboardLazy.disableShow == "false" || dashboardLazy.disableShow == null)
                    {
                        var Dashboard = await RequestAPI.getAsyncDashboard(dashBoards + "/" + dashboardLazy.dashboardConfigId);
                        lock (MemoryCaches.memDash)
                        {
                            if (MemoryCaches.memDash.Where(x => x.dashboardId == Dashboard.dashboardConfigId.ToString()).Count() == 0)
                                MemoryCaches.memDash.Add(new dashMemory
                                {
                                    active = true,
                                    dashboardId = Dashboard.dashboardConfigId.ToString(),
                                    dashboard = Dashboard
                                });
                            else
                            {
                                var cacheItem = MemoryCaches.memDash.Where(x => x.dashboardId == Dashboard.dashboardConfigId.ToString()).FirstOrDefault();
                                var index = MemoryCaches.memDash.IndexOf(cacheItem);
                                MemoryCaches.memDash[index] = new dashMemory
                                {
                                    active = cacheItem.active,
                                    dashboardId = Dashboard.dashboardConfigId.ToString(),
                                    dashboard = Dashboard
                                };
                            }
                        }
                    }
                    else
                    {
                        RequestAPI.DelAsyncCache(cacheURlMain + "/" + dashboardLazy.dashboardConfigId);
                    }
                }
                Logger.Info("------------------Dashboard Cache Updated------------------");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public async Task ThingsCache()
        {
            try
            {
                Dictionary<string, string> things = await RequestAPI.getAsyncThings(thingsURL);
                foreach (var thing in things)
                {
                    lock (MemoryCaches.memThings)
                    {
                        if (!MemoryCaches.memThings.ContainsKey((thing.Key)))
                            MemoryCaches.memThings.Add(thing.Key, thing.Value);
                        else
                            MemoryCaches.memThings[thing.Key] = thing.Value;
                    }
                }
                Logger.Info("------------------Things Cache Updated------------------");

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public async Task DisplayCache()
        {
            try
            {
                Dictionary<string, string> displays = await RequestAPI.getAsyncDisplayIPs(displayURL);
                foreach (var display in displays)
                {
                    lock (MemoryCaches.memMessages)
                    {
                        if (!MemoryCaches.memMessages.ContainsKey((display.Key)))
                            MemoryCaches.memMessages.Add(display.Key, display.Value);
                        else
                            MemoryCaches.memMessages[display.Key] = display.Value;
                    }
                }
                Logger.Info("------------------Display Cache Updated------------------");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public async Task DisableUnusedDash()
        {
            List<Task> tasks = new List<Task>();
            List<dashMemory> localDict;
            lock (MemoryCaches.memDash)
                localDict = new List<dashMemory>(MemoryCaches.memDash);
            foreach (var item in localDict)
            {
                var DashboardId = item.dashboardId;
                var lastRequest = await RequestAPI.getAsyncLastAccessCache(LastAccessURL + "/" + DashboardId);
                TimeSpan dif;
                if (lastRequest != null)
                {

                    dif = DateTime.Now - (DateTime)lastRequest;
                    Logger.Info("->" + item.dashboardId + " : " + item.dashboard.name);
                    Logger.Info("->Tempo desde Ultimo Acesso:" + dif);

                    if (dif.TotalMinutes > MaxTimeCache)
                    {
                        if (item.active == true)
                        {
                            lock (MemoryCaches.memDash)
                            {
                                MemoryCaches.memDash.Where(x => x.dashboardId == item.dashboardId).FirstOrDefault().active = false;
                            }

                            RequestAPI.DelAsyncCache(cacheURlMain + "/" + item.dashboardId);

                        }
                    }
                    else
                    {
                        if (item.active == false)
                        {
                            lock (MemoryCaches.memDash)
                            {
                                MemoryCaches.memDash.Where(x => x.dashboardId == item.dashboardId).FirstOrDefault().active = true;
                            }
                        }
                    }
                }

            }
        }

    }

}
