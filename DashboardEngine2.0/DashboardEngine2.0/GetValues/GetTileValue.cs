using DashboardEngine2._0.Model;
using DashboardEngine2._0.Requests;
using DashboardEngine2._0.StaticClasses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Classes
{
    class GetTileValue
    {
        private string server;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public GetTileValue(string server)
        {
            this.server = server;
        }
        public async Task<string> getTileValue(Tile tile, string dashboardid, bool PararellProcessTiles)
        {
            try
            {
                string url = null;
                string result = null;
                if (tile.dataSource != null)
                {
                    var startTime = DateTime.Now;

                    url = server + tile.dataSource.url;
                    if (tile.dataSource.queryParameters != null)
                    {
                        url += "?";
                        tile.dataSource.queryParameters = tile.dataSource.queryParameters.OrderByDescending(o => o.param).ToList();
                        for (int i = 0; i < tile.dataSource.queryParameters.Count; i++)
                        {
                            url += tile.dataSource.queryParameters[i].param.ToLower() + "=" + tile.dataSource.queryParameters[i].value.ToLower() + "&";
                        }
                    }
                    if (PararellProcessTiles)
                    {
                        result = await RequestAPI.processGetTile(url, tile.dataSource.path, true);
                        APIResponseTime.calculateResponseTime(tile.dataSource.url, DateTime.Now - startTime);
                    }
                    else
                    {
                        result = RequestAPISync.processGetTile(url, tile.dataSource.path, true);
                        APIResponseTime.calculateResponseTime(tile.dataSource.url, DateTime.Now - startTime);
                    }



                }
                if (result == null)
                    return "Error";
                else
                    return result; ;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString());
                return "Error";
            }
        }

    }
}
