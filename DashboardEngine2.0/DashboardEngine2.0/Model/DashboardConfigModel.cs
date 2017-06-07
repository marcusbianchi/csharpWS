using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Model
{

    public class DashboardHeader
    {
        public string headerDescription { get; set; }
        public string fontSize { get; set; }
        public string fontColor { get; set; }
        public string backgroundColor { get; set; }
        public object textDirection { get; set; }
    }

    public class QueryParameter
    {
        public string value { get; set; }
        public string param { get; set; }
    }

    public class DataSource
    {
        public string url { get; set; }
        public string path { get; set; }
        public List<QueryParameter> queryParameters { get; set; }
    }
    public class ColorSource
    {
        public string url { get; set; }
        public string path { get; set; }
        public string fontColorPath { get; set; }
        public List<QueryParameter> queryParameters { get; set; }
    }


    public class EmphasysSource
    {
        public string url { get; set; }
        public string path { get; set; }
        public List<QueryParameter> queryParameters { get; set; }
    }

    public class TextProperties
    {
        public string bold { get; set; }
        public string italic { get; set; }
        public string rotate { get; set; }
        public string fontSize { get; set; }
        public string fontColor { get; set; }
    }

    public class Border
    {
        public string style { get; set; }
        public string color { get; set; }
        public string witdh { get; set; }
    }


    public class GridHeader
    {
        public string headerContent { get; set; }
        public string width { get; set; }
        public string backgroundColor { get; set; }
        public TextProperties textProperties { get; set; }
    }


    public class GridRow
    {
        public string backgroundColor { get; set; }
        public TextProperties textProperties { get; set; }
    }

    public class Tile
    {
        public int? x { get; set; }
        public int? y { get; set; }
        public string area { get; set; }
        public bool? dynamicTable { get; set; }
        public List<int> mergeComponents { get; set; }
        public int tileId { get; set; }
        public bool show { get; set; }
        public string tileType { get; set; }
        public int? tileWidth { get; set; }
        public int? tileHeight { get; set; }
        public string detailsLink { get; set; }
        public string imageSource { get; set; }
        public string detailsLinkStyle { get; set; }
        public DataSource dataSource { get; set; }
        public ColorSource colorSource { get; set; }
        public EmphasysSource emphasysSource { get; set; }
        public string fixedValueCommon { get; set; }
        public string fixedValueUnique { get; set; }
        public TextProperties textProperties { get; set; }
        public string fixedColor { get; set; }
        public string tileContentType { get; set; }
        public string rail { get; set; }
        public Border border { get; set; }
        public int? rowQuantity { get; set; }
        public List<GridHeader> gridHeaders { get; set; }
        public GridRow gridRow { get; set; }
    }

    
    public class Dashboard
    {
        public int? dashboardConfigId { get; set; }
        public int? systemEndpointId { get; set; }
        public string name { get; set; }
        public string disableShow { get; set; }
        public string refreshRate { get; set; }
        public string gutter { get; set; }
        public string code { get; set; }
        public string bindingApi { get; set; }
        public string thingLevel { get; set; }
        public string bindingID { get; set; }
        public int? numberOfColumns { get; set; }
        public DashboardHeader dashboardHeader { get; set; }
        public int? tileRatio { get; set; }
        public bool? hasAxis { get; set; }
        public int? tileConfigId { get; set; }
        public List<string> things { get; set; }
        public List<Tile> tiles { get; set; }
    }  


}
