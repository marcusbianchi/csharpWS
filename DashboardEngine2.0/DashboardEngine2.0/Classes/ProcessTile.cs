using DashboardEngine2._0.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Classes
{
    class ProcessTile
    {
        private string server;
        private string errorColor;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ProcessTile(string server, string errorColor)
        {
            this.server = server;
            this.errorColor = errorColor;
        }
        public async Task<ModelShow.Tile> process(Tile tile, string dashboardID, bool PararellProcessTiles, bool PararellProcessGrid)
        {
            ModelShow.Tile tileshow = new ModelShow.Tile();
            tileshow = generateDefaultInfo(tile, tileshow);
            try
            {

                tileshow.tileType = tile.tileType;
                if (tile.tileType == "tile")
                {
                    tileshow = await processTile(tile, tileshow, dashboardID, PararellProcessTiles);
                }
                else if (tile.tileType == "image")
                {
                    tileshow.value = null;
                    tileshow.image = tile.imageSource;
                }
                else if (tile.tileType == "grid")
                {
                    ProcessGrid procGrid = new ProcessGrid(server, errorColor);
                    tileshow = await procGrid.generateGrid(tile, tileshow, dashboardID, PararellProcessTiles, PararellProcessGrid);
                }
                else
                {
                    tileshow.color = tile.fixedColor;
                    tileshow.value = tile.fixedValueCommon + " " + tile.fixedValueUnique;
                }

                return tileshow;
            }
            catch (Exception ex)
            {
                tileshow.show = tile.show;
                tileshow.tileHeight = tile.tileHeight;
                tileshow.tileWidth = tile.tileWidth;
                tileshow.color = errorColor;
                tileshow.value = ex.ToString();
                Logger.Error(ex.ToString());
                return tileshow;
            }
        }


      
        private ModelShow.Tile generateDefaultInfo(Tile tile, ModelShow.Tile tileshow)
        {
            tileshow.tileId = tile.tileId;
            tileshow.show = tile.show;
            tileshow.tileHeight = tile.tileHeight;
            tileshow.tileWidth = tile.tileWidth;
            tileshow.color = tile.fixedColor;
            tileshow.detailsLink = tile.detailsLink;
            tileshow.detailsLinkStyle = tile.detailsLinkStyle;
            tileshow.dynamicTable = tile.dynamicTable;
            tileshow.textProperties = new ModelShow.TextProperties();
            tileshow.border = new ModelShow.Border();
            tileshow.area = tile.area;
            if (tile.textProperties != null)
            {

                tileshow.textProperties.bold = tile.textProperties.bold;
                tileshow.textProperties.italic = tile.textProperties.italic;
                tileshow.textProperties.rotate = tile.textProperties.rotate;
                tileshow.textProperties.fontSize = tile.textProperties.fontSize;
                tileshow.textProperties.fontColor = tile.textProperties.fontColor;
            }
            else
            {

            }
            if (tile.border != null)
            {
                tileshow.border.color = tile.border.color;
                tileshow.border.style = tile.border.style;
                tileshow.border.witdh = tile.border.witdh;
            }
            return tileshow;
        }

        private async Task<ModelShow.Tile> processTile(Tile tile, ModelShow.Tile tileshow, string dashboardid, bool PararellProcessTiles)
        {
           
            tileshow.color = tile.fixedColor;
            if (tile.fixedValueCommon != null)
                tileshow.value = tile.fixedValueCommon + " ";
            if (tile.fixedValueUnique != null)
                tileshow.value = tile.fixedValueUnique;

            if (tile.colorSource != null)
            {
                GetTileColor processColor = new GetTileColor(server, errorColor);
                tileshow.color = await processColor.getTileColor(tile, PararellProcessTiles);
                if (tile.colorSource.fontColorPath != null)
                {
                    GetTileFontColor processFontColor = new GetTileFontColor(server, errorColor);
                    var resultColor = await processFontColor.getTileFontColor(tile, PararellProcessTiles);
                    if (resultColor == null)
                        tileshow.textProperties.fontColor = errorColor;
                    else
                        tileshow.textProperties.fontColor = resultColor;
                }
            }
            else
                tileshow.color = tile.fixedColor;

            if (tile.dataSource != null)
            {
                GetTileValue processValue = new GetTileValue(server);
                tileshow.value = await processValue.getTileValue(tile, dashboardid, PararellProcessTiles);

            }
            else
            {
                tileshow.value = "";
                if (tile.fixedValueCommon != null)
                    tileshow.value = tile.fixedValueCommon + " ";
                if (tile.fixedValueUnique != null)
                    tileshow.value += tile.fixedValueUnique;
            }
            if (tile.emphasysSource != null)
            {

                GetTileEmphasys processEmp = new GetTileEmphasys(server);
                var resultEmp = await processEmp.getTileEmphasys(tile, PararellProcessTiles);
                if (resultEmp == null)
                    tileshow.value = "ErroEmp";
                else
                    tileshow.emphasys = (bool)resultEmp;
            }
            else
                tileshow.emphasys = false;

            if (tile.rail != null)
            {
                tileshow.rail = tile.rail;
            }
            return tileshow;

        }
    }
}
