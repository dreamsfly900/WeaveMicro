using Newtonsoft.Json;
using System;
using wRPC;

namespace WeaveMicroClient
{
    public class MicroClient
    {
        Weave.TCPClient.P2Pclient P2Pclient = new Weave.TCPClient.P2Pclient(Weave.TCPClient.DataType.bytes);
        public delegate void receiveconfig(server[] serv);
        public event receiveconfig ReceiveEvent;
        String IP; int port;
        server serv;
        APIclient aPIclient;
        public MicroClient(String IP, int port)
        {
            this.IP = IP;this.port = port;
            P2Pclient.ReceiveServerEventbit += P2Pclient_ReceiveServerEventbit;
            P2Pclient.Timeoutevent += P2Pclient_Timeoutevent;
        }
        public void Stop()
        {
              P2Pclient.Stop();
        }
        public bool Connection()
        {
            return P2Pclient.Start(IP, port, false);
        }
        private void P2Pclient_Timeoutevent()
        {
            lb1122:
            if (!P2Pclient.Restart(false))
            {
                System.Threading.Thread.Sleep(1000);
                goto lb1122;
            }
            if (serv != null)
                RegService(serv);
            if (aPIclient != null)
                RegClient(aPIclient.Sid);
        }
        public void RegService(server serv)
        {
           String str= Newtonsoft.Json.JsonConvert.SerializeObject(serv);
            this.serv = serv;
            P2Pclient.Send(0x03, str);
        }
        public void SendLog(RouteLog serv)
        {
            String str = Newtonsoft.Json.JsonConvert.SerializeObject(serv);

            if (!P2Pclient.Send(0x02, str))
            {
                P2Pclient_Timeoutevent();
            }
            else
                Console.WriteLine($"发送:log-{str}");
        }
        public void RegClient(String Sid)
        {
            APIclient aPIclient = new APIclient();
            aPIclient.IP = IP;
            aPIclient.port = port;
            aPIclient.Sid = Sid;
            this.aPIclient = aPIclient;
               String str = Newtonsoft.Json.JsonConvert.SerializeObject(aPIclient);
            P2Pclient.Send(0x01, str);
        }
        private void P2Pclient_ReceiveServerEventbit(byte command, byte[] data)
        {
            if (command == 0x03)
            {
                try
                {
                    String datastr = System.Text.UTF8Encoding.UTF8.GetString(data);
                    if (ReceiveEvent != null)
                        ReceiveEvent(JsonConvert.DeserializeObject<server[]>(datastr));
                    //String datastr = System.Text.UTF8Encoding.UTF8.GetString(data);
                    //System.IO.StreamWriter sw = new System.IO.StreamWriter("funconfig.json", false);
                    //sw.Write(datastr);
                    //sw.Close();
                }
                catch(Exception e) 
                { throw  new  Exception( e.Message); }
            }
            
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

    }
    public class server
    {
        public String Name;
        public String IP { get; set; }
        public int Port { get; set; }
        public service[] services { get; set; }


    }

}
