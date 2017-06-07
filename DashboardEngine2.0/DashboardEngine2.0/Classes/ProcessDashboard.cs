using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashboardEngine2._0.Model;
using Newtonsoft.Json;
using AutoMapper;
using System.Configuration;
using System.Threading;
using DashboardEngine2._0.Requests;

namespace DashboardEngine2._0.Classes
{
    public class ProcessDashboard
    {
        private string server { get; set; }
        private string cacheURl { get; set; }
        private string errorColor { get; set; }
        private bool PararellProcessTiles { get; set; }
        private bool PararellProcessGrid { get; set; }


        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public ProcessDashboard(string server, string cacheURl, string errorColor, bool PararellProcessTiles, bool PararellProcessGrid)
        {
            this.server = server;
            this.cacheURl = cacheURl;
            this.errorColor = errorColor;
            this.PararellProcessTiles = PararellProcessTiles;
            this.PararellProcessGrid = PararellProcessGrid;
            Mapper.Initialize(cfg => cfg.CreateMap<ModelShow.Tile, Tile>());
        }

        public async Task processDashboard(Dashboard dashboard)
        {
            try
            {
                if (dashboard.dashboardConfigId == 289 || dashboard.dashboardConfigId == 286)
                    Console.Write("");
                var current = DateTime.Now;
                ModelShow.Dashboardshow dashShow = new ModelShow.Dashboardshow();
                dashShow.dashboardConfigId = dashboard.dashboardConfigId;
                dashShow.systemEndpointId = dashboard.systemEndpointId;
                dashShow.name = dashboard.name;
                dashShow.disableShow = dashboard.disableShow;
                dashShow.bindingApi = dashboard.bindingApi;
                dashShow.bindingID = dashboard.bindingID;
                dashShow.thingLevel = dashboard.thingLevel;
                dashShow.refreshRate = dashboard.refreshRate;
                dashShow.gutter = dashboard.gutter;
                dashShow.numberOfColumns = dashboard.numberOfColumns;
                dashShow.tileRatio = dashboard.tileRatio;
                dashShow.hasAxis = dashboard.hasAxis;
                dashShow.code = dashboard.code;
                dashShow.tileConfigId = dashboard.tileConfigId;
                dashShow.tiles = new List<ModelShow.Tile>();
                List<Task<ModelShow.Tile>> listTask;
                List<ModelShow.Tile> result;
                listTask = new List<Task<ModelShow.Tile>>();
                result = new List<ModelShow.Tile>();
                if (dashboard.tiles != null)
                {
                    foreach (Tile tile in dashboard.tiles)
                    {
                        Tile processtile = Mapper.Map<Tile>(tile);
                        ProcessTile TileProcesser = new ProcessTile(server, errorColor);
                        if (PararellProcessTiles)
                            listTask.Add(TileProcesser.process(processtile, dashboard.dashboardConfigId.ToString(), PararellProcessTiles, PararellProcessGrid));
                        else
                            result.Add(await TileProcesser.process(processtile, dashboard.dashboardConfigId.ToString(), PararellProcessTiles, PararellProcessGrid));
                    }
                    if (PararellProcessTiles)
                    {

                        var resulttasks = await Task.WhenAll(listTask);
                        dashShow.tiles = new List<ModelShow.Tile>(resulttasks.ToList());
                    }
                    else
                    {
                        dashShow.tiles = new List<ModelShow.Tile>(result);
                    }
                }
                if (dashboard.dashboardHeader != null)
                {
                    dashShow.dashboardHeader = new ModelShow.DashboardHeader();
                    dashShow.dashboardHeader.backgroundColor = dashboard.dashboardHeader.backgroundColor;
                    dashShow.dashboardHeader.fontColor = dashboard.dashboardHeader.fontColor;
                    dashShow.dashboardHeader.fontSize = dashboard.dashboardHeader.fontSize;
                    dashShow.dashboardHeader.headerDescription = dashboard.dashboardHeader.headerDescription;
                    dashShow.dashboardHeader.textDirection = dashboard.dashboardHeader.textDirection;
                }
                ProcessMessages procMes = new ProcessMessages(server);
                var messages = await procMes.getMessages(dashboard.things, dashboard.dashboardConfigId.ToString());
                if (messages != null)
                    dashShow.messages = new List<ModelShow.Message>(messages);
                Logger.Info(dashShow.dashboardConfigId + " " + dashShow.name + " Tempo " + ((DateTime.Now) - current));
                ModelShow.Cache cacheitem = new ModelShow.Cache();
                cacheitem.Content = JsonConvert.SerializeObject(dashShow);
                cacheitem.DashboardConfigId = (long)dashShow.dashboardConfigId;
                cacheitem.code = dashboard.code;
                List<ModelShow.Cache> cacheList = new List<ModelShow.Cache>();
                cacheList.Add(cacheitem);
                Thread postThread = new Thread(() => RequestAPI.PostAsyncCache(cacheURl, JsonConvert.SerializeObject(cacheList), dashboard.dashboardConfigId.ToString()));
                postThread.Start();
            }

            catch (Exception ex)
            {
                Logger.Info("Error: " + dashboard.dashboardConfigId + " " + dashboard.name);
                Logger.Error(ex.ToString());
            }
        }



    }
}
