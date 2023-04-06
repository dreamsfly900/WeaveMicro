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

        static String _Path = AppDomain.CurrentDomain.BaseDirectory;
        static String _currPath = Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用Weave微服务中心");
            var builder = new ConfigurationBuilder().SetBasePath(_Path).AddJsonFile("config.json");
            config = builder.Build();

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
            //  saveRouteLog();启用线程
            System.Threading.Thread goroute = new System.Threading.Thread(saveRouteLog);
            goroute.Start();

            weaveP2Server.Start(Convert.ToInt32(config["port"]));
            CreateHostBuilder(args).Build().Run();


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

        /// <summary>
        /// 保存日志
        /// </summary>
        private static void saveRouteLog()
        {
            if (!Directory.Exists(_Path + "route/"))
            {
                Directory.CreateDirectory(_Path + "route/");
            }
            while (true)
            {
                RouteLog[] routeLogs = new RouteLog[Routeloglist.Count];
                Routeloglist.CopyTo(0, routeLogs, 0, routeLogs.Length);
                if (Routeloglist.Count > 0)
                {
                    using (StreamWriter sw = new StreamWriter(_Path + "route/" + DateTime.Now.ToString("yyyyMMddHH") + "_log.json", true, Encoding.UTF8))
                    {
                        sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(routeLogs));
                        sw.Close();
                        sw.Dispose();
                    }
                    lock(Routeloglist)
                     Routeloglist.Clear();
                }
                System.Threading.Thread.Sleep(1000 * 60 );
            }
        }

        static List<server> servers = new List<server>();
        static List<APIclient> APIclientlist = new List<APIclient>();
        static List<APIclient> APIgateway = new List<APIclient>();
        static List<RouteLog> Routeloglist = new List<RouteLog>();
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
                        //Console.WriteLine($"网关info:{(client.socket.RemoteEndPoint as IPEndPoint).Address.ToString()}:{(client.socket.RemoteEndPoint as IPEndPoint).Port} {(client.socket.LocalEndPoint as IPEndPoint).Address.ToString()}:{(client.socket.LocalEndPoint as IPEndPoint).Port}");
                        Console.WriteLine($"网关加入 {client.Sid} {client.IP}:{client.port}");

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

                        savegateway();
                        post();
                        break;
                    case 0x02:
                        //类型2

                        RouteLog rl = Newtonsoft.Json.JsonConvert.DeserializeObject<RouteLog>(System.Text.UTF8Encoding.UTF8.GetString(data));
                        Console.WriteLine($"网关：{rl.gayway},请求:{rl.RouteIP}:{rl.Route}，请求IP:{rl.requestIP},耗时:{rl.time}毫秒");
                        if (rl != null)
                        {
                            Routeloglist.Add(rl);
                        }
                        break;
                    case 0x03:
                        //类型3

                        server sers = Newtonsoft.Json.JsonConvert.DeserializeObject<server>(System.Text.UTF8Encoding.UTF8.GetString(data));
                        Console.WriteLine($"服务加入{sers.Name} {sers.IP}:{sers.Port}");
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
            //服务
            try
            {
                System.IO.StreamReader sw = new StreamReader(_Path + "temp.json");
                String data = sw.ReadToEnd();
                sw.Close();
                List<server> ser = Newtonsoft.Json.JsonConvert.DeserializeObject<List<server>>(data);
                return ser;
            }
            catch { }

            //网关
            try
            {
                System.IO.StreamReader sw = new StreamReader(_Path + "gateway.json");
                String data = sw.ReadToEnd();
                sw.Close();
                if (data != "")
                {
                    APIclientlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<APIclient>>(data);
                }
            }
            catch { }

            return null;
        }

        static void save(String str)
        {
            try
            {
                System.IO.StreamWriter sw = new StreamWriter(_Path + "temp.json");
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
                System.IO.StreamWriter sw = new StreamWriter(_Path + "gateway.json");
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
