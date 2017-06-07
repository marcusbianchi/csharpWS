using DashboardEngine2._0.Model;
using DashboardEngine2._0.Requests;
using DashboardEngine2._0.StaticClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Classes
{
    class GetTileGrid
    {
        private string server;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public GetTileGrid(string server)
        {
            this.server = server;
        }
        public async Task<List<string[]>> getGridValues(Tile tile,string dashboardID, bool PararellProcessGrid)
        {
            try
            {
                string url = null;
                List<string[]> result = null;
                if (tile.dataSource != null)
                {
                    var startTime = DateTime.Now;

                    url = server + tile.dataSource.url;
                    url += "?";
                    tile.dataSource.queryParameters = tile.dataSource.queryParameters.OrderBy(o => o.param).ToList();
                    for (int i = 0; i < tile.dataSource.queryParameters.Count; i++)
                    {
                        url += tile.dataSource.queryParameters[i].param.ToLower() + "=" + tile.dataSource.queryParameters[i].value.ToLower() + "&";
                    }

                    if (PararellProcessGrid)
                    {
                        result = await RequestAPI.getAsyncGrid(url, dashboardID);
                        APIResponseTime.calculateResponseTime(tile.dataSource.url, DateTime.Now - startTime);
                    }
                    else
                    {
                        result = RequestAPISync.getSyncGrid(url, dashboardID);
                        APIResponseTime.calculateResponseTime(tile.dataSource.url, DateTime.Now - startTime);
                    }
                   

                }
                return result; ;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString());

                return null;
            }
        }
    }
}
