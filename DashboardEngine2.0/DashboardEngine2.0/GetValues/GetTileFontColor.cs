using DashboardEngine2._0.Model;
using DashboardEngine2._0.Requests;
using DashboardEngine2._0.StaticClasses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Classes
{

    class GetTileFontColor
    {
        private string server;
        private string errorColor;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public GetTileFontColor(string server, string errorColor)
        {
            this.server = server;
            this.errorColor = errorColor;
        }
        public async Task<string> getTileFontColor(Tile tile,bool PararellProcessTiles)
        {
            try
            {
                string url = null;
                string result = null;
                if (tile.colorSource != null)
                {
                    var startTime = DateTime.Now;
                    tile.colorSource.queryParameters = tile.colorSource.queryParameters.OrderBy(o => o.param).ToList();
                    url = server + tile.colorSource.url;
                    url += "?";
                    for (int i = 0; i < tile.colorSource.queryParameters.Count; i++)
                    {
                        url += tile.colorSource.queryParameters[i].param.ToLower() + "=" + tile.colorSource.queryParameters[i].value.ToLower() + "&";
                    }
                    if (PararellProcessTiles)
                    {
                        result = await RequestAPI.processGetTile(url, tile.colorSource.fontColorPath, true);
                        APIResponseTime.calculateResponseTime(tile.colorSource.url, DateTime.Now - startTime);
                    }
                    else
                    {
                        result = RequestAPISync.processGetTile(url, tile.colorSource.fontColorPath, true);
                        APIResponseTime.calculateResponseTime(tile.colorSource.url, DateTime.Now - startTime);
                    }
                }
                if (result == null)
                    return errorColor;
                else
                    return result;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString());
                return errorColor;
            }
        }
    }
}
