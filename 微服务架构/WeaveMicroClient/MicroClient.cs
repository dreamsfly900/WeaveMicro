using System;

namespace WeaveMicroClient
{
    public class MicroClient
    {
        Weave.TCPClient.P2Pclient P2Pclient = new Weave.TCPClient.P2Pclient(Weave.TCPClient.DataType.bytes);
        String IP; int port;
        public MicroClient(String IP, int port)
        {
            this.IP = IP;this.port = port;
            P2Pclient.ReceiveServerEventbit += P2Pclient_ReceiveServerEventbit;
            P2Pclient.Timeoutevent += P2Pclient_Timeoutevent;
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
        }
        public void RegService(server serv)
        {
           String str= Newtonsoft.Json.JsonConvert.SerializeObject(serv);
            P2Pclient.Send(0x03, str);
        }
        public void RegClient(String Sid)
        {
            APIclient aPIclient = new APIclient();
            aPIclient.IP = IP;
            aPIclient.port = port;
            aPIclient.Sid = Sid;
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
                    System.IO.StreamWriter sw = new System.IO.StreamWriter("config.json", false);
                    sw.Write(datastr);
                    sw.Close();
                }
                catch { }
            }
            
        }
    }
    public class APIclient
    {
        public string Sid { get; set; }
        public string IP { get; set; }
        public int port { get; set; }

    }
    public class server
    {
         

        public String IP { get; set; }
        public int Port { get; set; }
        public service[] services { get; set; }
 
    }
    public class service
    {
        public string Route { get; set; }
        public string Method { get; set; }
        public String[] parameter { get; set; }
        public string annotation { get; set; }
    }
}
