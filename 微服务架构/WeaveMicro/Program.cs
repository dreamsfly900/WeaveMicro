using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using WeaveMicrocenter;
using wRPCclient;
using System.Linq;

namespace WeaveMicro
{
    class Program
    {
        static Weave.Server.WeaveP2Server weaveP2Server = new Weave.Server.WeaveP2Server(Weave.Base.WeaveDataTypeEnum.Bytes);
        static IConfigurationRoot config = null;
        static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用Weave微服务中心");
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json");
            config = builder.Build();

            CreateHostBuilder(args).Build().Run();

            weaveP2Server.weaveReceiveBitEvent += WeaveP2Server_weaveReceiveBitEvent;
            weaveP2Server.weaveDeleteSocketListEvent += WeaveP2Server_weaveDeleteSocketListEvent;
            List<server> tempservers = GetServers("temp.json");
            for (int i = 0; i < servers.Count; i++)
            {
                ClientChannel channel = new ClientChannel(servers[i].IP, servers[i].Port);
                if (channel.IsLine())
                {
                    servers.Add(servers[i]);
                }
            }
        
            weaveP2Server.Start(Convert.ToInt32(config["port"]));
            while (true)
            {
                string command = Console.ReadLine();
                switch (command)
                {
                    case "printC":
                        lock (APIclientlist)
                        {

                            foreach (APIclient api in APIclientlist)
                            {
                                Console.WriteLine($"网关:{(api.socket.RemoteEndPoint as IPEndPoint).Address.ToString()}+{(api.socket.RemoteEndPoint as IPEndPoint).Port}");

                            }
                        }
                        break;
                    case "printS":
                        lock (servers)
                        {
                            foreach (server ser in servers)
                            {

                                Console.WriteLine($"服务:{ser.IP}:{ser.Port}");

                            }
                        }
                        break;
                    case "exit":
                        Environment.Exit(0);
                        return;
                    default:
                        Console.WriteLine("输入的有误，请重新输入");
                        break;
                }
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>

          Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseUrls(config["url"]).UseStartup<Startup>();
               });
        private static void WeaveP2Server_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            try
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
                lock (APIgateway)
                {

                    foreach (APIclient api in APIgateway)
                    {
                        if (api.socket.Equals(soc))
                        {
                            APIgateway.Remove(api);
                            lock (servers)
                            {
                                foreach (server ser in servers)
                                {
                                    if (api.IP == ser.IP && api.port == ser.Port)
                                    {
                                        servers.Remove(ser);
                                        break;
                                    }

                                }
                            }

                            post();
                            return;
                        }

                    }
                }
            }
            catch { }
        }

        static List<server> servers = new List<server>();
        static List<APIclient> APIclientlist = new List<APIclient>();
        static List<APIclient> APIgateway = new List<APIclient>();
        private static void WeaveP2Server_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            try
            {
                Console.WriteLine($"{command}");
                switch (command)
                {
                    case 0x01:
                        //类型1
                        APIclient client = Newtonsoft.Json.JsonConvert.DeserializeObject<APIclient>(System.Text.UTF8Encoding.UTF8.GetString(data));
                        
                        client.socket = soc;
                        lock (APIclientlist)
                        {
                            foreach (APIclient ser in APIclientlist)
                            {
                                if (client.IP == ser.IP && client.port == ser.port)
                                {
                                    APIclientlist.Remove(ser); 
                                    break;
                                }
                            }
                        }
                        APIclientlist.Add(client);
                        Console.WriteLine($"网关加入:{client.IP}+{client.port}");

                        savegateway();
                        post();
                        break;
                    case 0x02:
                        //类型2

                        RouteLog rl = Newtonsoft.Json.JsonConvert.DeserializeObject<RouteLog>(System.Text.UTF8Encoding.UTF8.GetString(data));
                        Console.WriteLine($"网关：{rl.gayway},请求:{rl.RouteIP}+{rl.Route}，请求IP:{rl.requestIP},耗时:{rl.time}毫秒");

                        break;
                    case 0x03:
                        //类型3

                        server sers = Newtonsoft.Json.JsonConvert.DeserializeObject<server>(System.Text.UTF8Encoding.UTF8.GetString(data));
                        Console.WriteLine($"服务加入{sers.IP}:{sers.Port}");
                        lock (servers)
                        {
                            foreach (server ser in servers)
                            {
                                if (sers.IP == ser.IP && sers.Port == ser.Port)
                                {
                                    servers.Remove(ser);
                                    break;
                                }

                            }
                        }
                        servers.Add(sers);
                        APIclient aPIclient = new APIclient();
                        aPIclient.socket = soc;
                        aPIclient.IP = sers.IP;
                        aPIclient.port = sers.Port;
                        APIgateway.Add(aPIclient);
                        save(JsonConvert.SerializeObject(servers));
                        post();
                        break;
                    default:
                        Console.WriteLine("输入的有误，请重新输入");
                        break;
                }
            }
            catch (Exception e) { Console.WriteLine("weaveReceiveBitEvent" + e.Message); }
        }

        static List<server> GetServers(String file)
        {
            //网关
            try
            {
                System.IO.StreamReader sw = new StreamReader("gateway.json");
                String data = sw.ReadToEnd();
                sw.Close();
                if (data != "")
                {
                    APIclientlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<APIclient>>(data);
                }
            }
            catch { }
            //服务
            try
            {
                System.IO.StreamReader sw = new StreamReader("server.json");
                String data = sw.ReadToEnd();
                sw.Close();
                List<server> ser = Newtonsoft.Json.JsonConvert.DeserializeObject<List<server>>(data);
                return ser;
            }
            catch { }
            return null;
        }

        static void save(String str)
        {
            try
            {
                System.IO.StreamWriter sw = new StreamWriter("server.json");
                sw.Write(str);
                sw.Close();
            }
            catch { }

        }
        static void savegateway()
        {
            List<APIclient> temp = APIclientlist;
            String allstr = Newtonsoft.Json.JsonConvert.SerializeObject(temp);
            try
            {
                System.IO.StreamWriter sw = new StreamWriter("gateway.json");
                sw.Write(allstr);
                sw.Close();
            }
            catch { }

        }
        static void post()
        {
            String allstr = Newtonsoft.Json.JsonConvert.SerializeObject(servers);           

            foreach (APIclient api in APIclientlist)
            {

                weaveP2Server.Send(api.socket, 0x03, UTF8Encoding.UTF8.GetBytes(allstr));

            }
        }
    }
}
