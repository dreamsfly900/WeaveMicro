using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using WeaveMicrocenter;

namespace WeaveMicro
{
    class Program
    {
       static Weave.Server.WeaveP2Server weaveP2Server = new Weave.Server.WeaveP2Server(Weave.Base.WeaveDataTypeEnum.Bytes);
        static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用Weave微服务中心");
            weaveP2Server.weaveReceiveBitEvent += WeaveP2Server_weaveReceiveBitEvent;
            weaveP2Server.weaveDeleteSocketListEvent += WeaveP2Server_weaveDeleteSocketListEvent;
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json");


            var config = builder.Build();
            weaveP2Server.Start(Convert.ToInt32( config["config"]));
            while (true)
            {
                string command=Console.ReadLine();
                switch (command) 
                {
                    case "exit":
                        Environment.Exit(0);
                        return; 
                    default:
                        Console.WriteLine("输入的有误，请重新输入"); 
                        break;
                }
            }
        }

        private static void WeaveP2Server_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            lock (APIclientlist)
            {

                foreach (APIclient api in APIclientlist)
                {
                    if (api.socket.Equals(soc))
                    {
                        APIclientlist.Remove(api);
                        return;
                    }

                }
            }
        }

        static List<server> servers = new List<server>();
        static List<APIclient> APIclientlist = new List<APIclient>();
        private static void WeaveP2Server_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            switch (command)
            {
                case  0x01:
                    //类型1
                    APIclient client = Newtonsoft.Json.JsonConvert.DeserializeObject<APIclient>(System.Text.UTF8Encoding.UTF8.GetString(data));
                    client.socket = soc;
                    APIclientlist.Add(client);
                    break;
                case 0x02:
                    //类型2
                    break;
                case 0x03:
                    //类型2
                    server ser= Newtonsoft.Json.JsonConvert.DeserializeObject<server>(System.Text.UTF8Encoding.UTF8.GetString(data));
                    servers.Add(ser);
                   String allstr= Newtonsoft.Json.JsonConvert.SerializeObject(servers);
                    foreach (APIclient api in APIclientlist)
                    {
                        
                        weaveP2Server.Send(api.socket, 0x03, UTF8Encoding.UTF8.GetBytes(allstr));

                    }
                        break;
                default:
                    Console.WriteLine("输入的有误，请重新输入");
                    break;
            }
        }
    }
}
