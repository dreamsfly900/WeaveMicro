using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using wRPC;

namespace WeaveMicrocenter
{
   public class Manage
    {
        List<APIclient> APIclients = new List<APIclient>();
        public static bool regclient(string Sid,string IP,int port)
        {

            return false;

        }
        public static bool regApi(string API,params string[] Params)
        {

            return false;

        }
        public static bool regApiclient(string Sid, string IP, int port)
        {

            return false;

        }
    }
    public class RouteLog
    {
        public string gayway;
        public string RouteIP;
        public string Route;
        public string requestIP;
        public string time;

    }
    public class APIclient
    {
        public string Sid { get; set; }
        public string IP { get; set; }
        public int port { get; set; }
        [JsonIgnore]
        public Socket socket { get; set; }
    }
    public class server
    {
        public String Name { get; set; }
        public String IP { get; set; }
        public int Port { get; set; }
        public service[] services { get; set; }


    }
    
}
