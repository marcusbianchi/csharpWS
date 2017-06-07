using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardEngine2._0.ModelShow
{

    public class DashboardHeader
    {
        public string headerDescription { get; set; }
        public string fontSize { get; set; }
        public string fontColor { get; set; }
        public string backgroundColor { get; set; }
        public object textDirection { get; set; }
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

   
    public class Header
    {
        public string headerContent { get; set; }
        public string width { get; set; }
        public string backgroundColor { get; set; }
        public TextProperties textProperties { get; set; }
    }

   

    public class Row
    {
        public List<string> rowContent { get; set; }
        public string backgroundColor { get; set; }
        public TextProperties textProperties { get; set; }
    }

    public class Tile
    {
        public int tileId { get; set; }
        public bool? dynamicTable { get; set; }
        public string area { get; set; }

        public int? rowQuantity { get; set; }
        public string tileType { get; set; }
        public bool show { get; set; }
        public int? tileWidth { get; set; }
        public int? tileHeight { get; set; }
        public string detailsLink { get; set; }
        public object value { get; set; }
        public string color { get; set; }
        public bool emphasys { get; set; }
        public string rail { get; set; }
        public string detailsLinkStyle { get; set; }
        public string image { get; set; }

        public TextProperties textProperties { get; set; }
        public Border border { get; set; }
        
        
        
    }
    public class gridValue
    {
        public List<Header> headers { get; set; }
        public List<Row> rows { get; set; }
        public int? gridCount { get; set; }
    }

    public class Dashboardshow
    {
        public int? dashboardConfigId { get; set; }
        public int? systemEndpointId { get; set; }
        public string name { get; set; }
        public string disableShow { get; set; }
        public string tileContentType { get; set; }
        public object refreshRate { get; set; }
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
        public List<Tile> tiles { get; set; }
        public List<Message> messages { get; set; }
    }
    public class Message
    {
        public List<string> messageList { get; set; }
        public string ip { get; set; }
    }
}
