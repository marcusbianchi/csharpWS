using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardEngine2._0.AuxClasses
{
    public class ConnectionManagerDataSection : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "ConnectionManagerDataSection";

        private const string EndpointCollectionName = "ConnectionManagerEndpoints";

        [ConfigurationProperty(EndpointCollectionName)]
        [ConfigurationCollection(typeof(ConnectionManagerEndpointsCollection), AddItemName = "add")]
        public ConnectionManagerEndpointsCollection ConnectionManagerEndpoints { get { return (ConnectionManagerEndpointsCollection)base[EndpointCollectionName]; } }
    }

    public class ConnectionManagerEndpointsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionManagerEndpointElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConnectionManagerEndpointElement)element).dashboardId;
        }
    }

    public class ConnectionManagerEndpointElement : ConfigurationElement
    {
        [ConfigurationProperty("messageUrl", IsRequired = true)]
        public string messageUrl
        {
            get { return (string)this["messageUrl"]; }
            set { this["messageUrl"] = value; }
        }

        [ConfigurationProperty("messagePath", IsRequired = true)]
        public string messagePath
        {
            get { return (string)this["messagePath"]; }
            set { this["messagePath"] = value; }
        }

        [ConfigurationProperty("dashboardId", IsRequired = true)]
        public string dashboardId
        {
            get { return (string)this["dashboardId"]; }
            set { this["dashboardId"] = value; }
        }    
       
    }
}
