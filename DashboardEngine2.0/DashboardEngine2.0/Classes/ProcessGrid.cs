using DashboardEngine2._0.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Classes
{
    class ProcessGrid
    {
        private string server;
        private string errorColor;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ProcessGrid(string server, string errorColor)
        {
            this.server = server;
            this.errorColor = errorColor;
        }
        public async Task<ModelShow.Tile> generateGrid(Tile tile, ModelShow.Tile tileshow, string dashboardID, bool PararellProcessTiles, bool PararellProcessGrid)
        {
            try
            {
                if (dashboardID == "285")
                    Console.Write("");
                ModelShow.gridValue gValue = new ModelShow.gridValue();
                gValue.headers = new List<ModelShow.Header>();
                tileshow.rowQuantity = tile.rowQuantity;
                if (tile.gridHeaders != null)
                {
                    foreach (var item in tile.gridHeaders)
                    {
                        ModelShow.Header header = new ModelShow.Header();
                        header.backgroundColor = item.backgroundColor;
                        header.headerContent = item.headerContent;
                        if (item.textProperties != null)
                        {
                            header.textProperties = new ModelShow.TextProperties();
                            header.textProperties.bold = item.textProperties.bold;
                            header.textProperties.fontColor = item.textProperties.fontColor;
                            header.textProperties.fontSize = item.textProperties.fontSize;
                            header.textProperties.italic = item.textProperties.italic;
                            header.textProperties.rotate = item.textProperties.rotate;
                        }
                        //----------------------------------CICERO-------------------------------------------
                        header.width = item.width;
                        //-------------------------------------------------------------------------------------

                        gValue.headers.Add(header);
                    }
                }

                GetTileGrid proceesGrid = new GetTileGrid(server);
                var gridValues = await proceesGrid.getGridValues(tile, dashboardID, PararellProcessGrid);
                if (gridValues == null)
                    gValue.gridCount = 0;
                else
                    gValue.gridCount = gridValues.Count();
                gValue.rows = new List<ModelShow.Row>();
                if (gridValues == null)
                {
                    for (int i = 0; i < tile.rowQuantity + 1; i++)
                    {
                        ModelShow.Row error = new ModelShow.Row();
                        error.backgroundColor = errorColor;
                        error.rowContent = new List<string>();
                        error.rowContent.Add("Error");
                        error.textProperties = new ModelShow.TextProperties();
                        if (tile.gridRow.textProperties != null)
                        {
                            error.textProperties.bold = tile.gridRow.textProperties.bold;
                            error.textProperties.fontColor = tile.gridRow.textProperties.fontColor;
                            error.textProperties.fontSize = tile.gridRow.textProperties.fontSize;
                            error.textProperties.italic = tile.gridRow.textProperties.italic;
                            error.textProperties.rotate = tile.gridRow.textProperties.rotate;
                        }
                        gValue.rows.Add(error);
                    }
                }
                else
                {
                    int? remainder = 0;
                    int size = 0;
                    if (tile.dynamicTable == null)
                        tile.dynamicTable = false;

                    if (tile.dynamicTable == true)
                    {
                        size = gridValues.Count;
                        if (size > tileshow.rowQuantity)
                        {
                            remainder = size % tileshow.rowQuantity;
                            if (remainder != null && remainder != 0)
                                size = size + ((int)tileshow.rowQuantity - (int)remainder);
                        }
                        else
                        {
                            size = (int)tileshow.rowQuantity;                           
                        }
                    }
                    else
                        size = (int)tile.rowQuantity;

                    if(dashboardID== "270")
                            Console.Write("");

                    for (int i = 0; i < size; i++)
                    {
                        ModelShow.Row row = new ModelShow.Row();

                        if (i >= gridValues.Count)
                            row.rowContent = new List<string>();
                        //----------------------------------CICERO-------------------------------------------
                        else
                        {
                            row.rowContent = gridValues[i].Select(x => x == "" ? "-" : x).ToList();
                        }
                        //-------------------------------------------------------------------------------------
                        row.textProperties = new ModelShow.TextProperties();
                        if (tile.gridRow!=null && tile.gridRow.textProperties != null)
                        {
                            row.textProperties.bold = tile.gridRow.textProperties.bold;
                            row.textProperties.fontColor = tile.gridRow.textProperties.fontColor;
                            row.textProperties.fontSize = tile.gridRow.textProperties.fontSize;
                            row.textProperties.italic = tile.gridRow.textProperties.italic;
                            row.textProperties.rotate = tile.gridRow.textProperties.rotate;
                        }
                        gValue.rows.Add(row);

                    }
                }
                string color= "#ffffff";
                string fontColor= "#000000";
                if (tile.gridRow != null)
                {
                    if (tile.gridRow.backgroundColor == null)
                        tile.gridRow.backgroundColor = "#ffffff";
                    if (tile.gridRow.textProperties.fontColor == null)
                        tile.gridRow.textProperties.fontColor = "#000000";
                    color = tile.gridRow.backgroundColor;
                    fontColor = tile.gridRow.textProperties.fontColor;
                }
                if (tile.colorSource != null)
                {
                    GetTileColor processColor = new GetTileColor(server, errorColor);
                    color = await processColor.getTileColor(tile, PararellProcessTiles);
                    if (tile.colorSource.fontColorPath != null)
                    {
                        GetTileFontColor processFontColor = new GetTileFontColor(server, errorColor);
                        var resultColor = await processFontColor.getTileFontColor(tile, PararellProcessTiles);
                        if (resultColor == null)
                            fontColor = errorColor;
                        else
                            fontColor = resultColor;
                    }
                }

                foreach (var row in gValue.rows)
                {
                    row.backgroundColor = color;
                    row.textProperties.fontColor = fontColor;
                }
                tileshow.value = gValue;

                return tileshow;
            }
            catch (Exception ex)
            {
                Logger.Error(dashboardID);
                Logger.Error(ex.ToString());
                return null;
            }
        }
    }
}
