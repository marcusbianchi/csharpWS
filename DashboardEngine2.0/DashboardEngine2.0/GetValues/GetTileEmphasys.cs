using DashboardEngine2._0.Model;
using DashboardEngine2._0.Requests;
using DashboardEngine2._0.StaticClasses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Classes
{
    class GetTileEmphasys
    {
        private string server;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public GetTileEmphasys(string server)
        {
            this.server = server;
        }
        public async Task<bool?> getTileEmphasys(Tile tile, bool PararellProcessTiles)
        {
            try
            {
                
                string url = null;
                string result = null;
                if (tile.emphasysSource != null)
                {
                    url = server + tile.emphasysSource.url;
                    url += "?";
                    tile.emphasysSource.queryParameters = tile.emphasysSource.queryParameters.OrderBy(o => o.param).ToList();
                    var startTime = DateTime.Now;
                    for (int i = 0; i < tile.emphasysSource.queryParameters.Count; i++)
                    {
                        url += tile.emphasysSource.queryParameters[i].param.ToLower() + "=" + tile.emphasysSource.queryParameters[i].value.ToLower() + "&";
                    }
                    if (PararellProcessTiles)
                    {
                        result = await RequestAPI.processGetTile(url, tile.emphasysSource.path, true);
                        APIResponseTime.calculateResponseTime(tile.emphasysSource.url, DateTime.Now - startTime);
                    }
                    else
                    {
                        result = RequestAPISync.processGetTile(url, tile.emphasysSource.path, true);
                        APIResponseTime.calculateResponseTime(tile.emphasysSource.url, DateTime.Now - startTime);
                    }
                }
                var boolRes = Convert.ToBoolean(result);
             

                return boolRes;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString());
                return null;
            }
        }
    }
}
