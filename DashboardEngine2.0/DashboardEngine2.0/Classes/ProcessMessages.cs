using DashboardEngine2._0.AuxClasses;
using DashboardEngine2._0.ModelShow;
using DashboardEngine2._0.Requests;
using DashboardEngine2._0.StaticClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardEngine2._0.Classes
{
    public class FilterStatus
    {
        public List<int> thingIds { get; set; }
        public string[] states { get; set; }
    }

    class ProcessMessages
    {
        private string server;
        private string[] status;
        private string statesBatchAddress;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProcessMessages(string server)
        {
            this.server = server;
            status = System.Configuration.ConfigurationSettings.AppSettings["statesOnMessages"].ToString().Split(',');
            statesBatchAddress = System.Configuration.ConfigurationSettings.AppSettings["statesBatchAddress"].ToString();
        }

        public async Task<List<Message>> getMessages(List<string> things, string DashboardID)
        {
            if (DashboardID == "223")
                Console.Write("");
            List<Message> listMess = new List<Message>();
            FilterStatus filter = new FilterStatus();
            Dictionary<string, string> messagesForDashboard;
            List<string> MessageForAll = new List<string>();
            Message defaultMes = new Message();
            Dictionary<string, string> localMesssages;
            try
            {
                things = things.Where(x => x != null).ToList();
                var startTime = DateTime.Now;
                if (things.Count != 0)
                {
                    filter.thingIds = things.Select(int.Parse).ToList();
                    filter.states = status;
                    messagesForDashboard = await RequestAPI.getAsyncMessages(server + statesBatchAddress, filter, DashboardID);
                    if (messagesForDashboard != null)
                    {
                        foreach (var message in messagesForDashboard)
                        {
                            string thingCode;

                            lock (MemoryCaches.memThings)
                            {
                                if (MemoryCaches.memThings.ContainsKey(message.Key))
                                {
                                    thingCode = (string)MemoryCaches.memThings[message.Key];
                                    MessageForAll.Add(thingCode + " - " + message.Value.Replace("_", " "));

                                }
                            }


                        }
                    }
                }

                MessageForAll.Sort();
                APIResponseTime.calculateResponseTime(server + statesBatchAddress, DateTime.Now - startTime);
                defaultMes.ip = "default";
                if (MessageForAll.Count != 0)
                    defaultMes.messageList = MessageForAll;
                else
                    defaultMes.messageList = new List<string>();
                listMess.Add(defaultMes);
                lock (MemoryCaches.memMessages)
                {
                    if (MemoryCaches.memMessages != null)
                        localMesssages = new Dictionary<string, string>(MemoryCaches.memMessages);
                    else
                        localMesssages = new Dictionary<string, string>();

                    foreach (var display in localMesssages)
                    {
                        Message newMess = new Message();
                        newMess.ip = display.Key.ToString();
                        if (MessageForAll.Count != 0)
                            newMess.messageList = new List<string>(MessageForAll);
                        else
                            newMess.messageList = new List<string>();
                        newMess.messageList.Add(display.Value.ToString());
                        listMess.Add(newMess);
                    }
                }
                listMess = await getMessagesfromEndpoints(listMess, DashboardID);
                return listMess;
            }
            catch (Exception ex)
            {
                Logger.Error(DashboardID + " " + ex.ToString());
                return null;
            }
        }

        public async Task<List<Message>> getMessagesfromEndpoints(List<Message> listMess, string DashboardID)
        {
            var connectionManagerDataSection = ConfigurationManager.GetSection(ConnectionManagerDataSection.SectionName) as ConnectionManagerDataSection;

            foreach (ConnectionManagerEndpointElement endpointElement in connectionManagerDataSection.ConnectionManagerEndpoints)
            {
                if (DashboardID == endpointElement.dashboardId)
                {
                    GetTileValue processValue = new GetTileValue(server);

                    var result = await RequestAPI.processGetTile(endpointElement.messageUrl, endpointElement.messagePath, true);
                    foreach (var item in listMess)
                    {
                        item.messageList.Add(result);
                    }
                }
            }
            return listMess;
        }

    }
}
