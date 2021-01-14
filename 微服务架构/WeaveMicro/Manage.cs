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
    public class APIclient
    {
        public string Sid { get; set; }
        public string IP { get; set; }
        public int port { get; set; }
        public Socket socket { get; set; }
    }
    public class server
    {

        public String IP { get; set; }
        public int Port { get; set; }
        public service[] services { get; set; }


    }
    
}
