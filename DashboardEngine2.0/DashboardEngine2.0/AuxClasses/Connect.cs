using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DashboardEngine2._0.AuxClasses
{
    class Connect
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string HttpGet(string uri, int timeout)
        {
            try
            {
                MyWebClient client = new MyWebClient(timeout);

                // Add a user agent header in case the 
                // requested URI contains a query.
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                Stream data = client.OpenRead(uri);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();

                return s;
            }
            catch(Exception ex)
            {
                Logger.Error("GET: " + uri);
                Logger.Error(ex.ToString());
                throw ex;
            }
        }

   
    }

    public class MyWebClient : WebClient
    {
        private int timeout;
        public MyWebClient(int timeout): base()
        {
            this.timeout = timeout;
        }
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = timeout;
            return w;
        }
    }
}
