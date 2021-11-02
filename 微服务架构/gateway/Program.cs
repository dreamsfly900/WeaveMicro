using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WeaveMicroClient;
using wRPC;

namespace gateway
{
    class Program
    {
        public static MicroClient mc;
        public static string applicationUrl;
        static void Main(string[] args)
        {

            //server[] servers= Funconfig.getConfig();
            // server ser= WeightAlgorithm.Get(servers, "abcd/ff");
            Console.WriteLine("Running WeaveMicro网关.");
            var config = builder.Build();
            String mcip = config["Microcenter"];
            mc = new MicroClient(mcip.Split(':')[0], Convert.ToInt32(mcip.Split(':')[1]));
            mc.ReceiveEvent += Mc_ReceiveEvent;
            mc.Connection();
            String[] applicationUrls = config["applicationUrl"].Replace("http://","").Replace("https://", "").Split(',');
            foreach(string applicationUrl in applicationUrls)
            mc.RegClient("网关1", applicationUrl.Split(':')[0], Convert.ToInt32( applicationUrl.Split(':')[1]));
            applicationUrl = config["applicationUrl"];
            args = new string[] { config["applicationUrl"] };
            CreateHostBuilder(args).Build().Run();

            //mainthread loop
            while (true)
            {
                System.Threading.Thread.Sleep(10);
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "exit":
                        break;
                    default:
                        continue;
                }
            }
            mc.Stop();


        }

        private static void Mc_ReceiveEvent(WeaveMicroClient.server[] serv)
        {
            try
            {
                String datastr = Newtonsoft.Json.JsonConvert.SerializeObject(serv);

                datastr = "{\"server\":" + datastr + "}";
                System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory+"funconfig.json", false);
                sw.Write(datastr);
                sw.Close();
                Proccessor.servers = Funconfig.getConfig();
                WeightAlgorithm._serviceDic.Clear();


            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("config.json");

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>

            WebHost.CreateDefaultBuilder().UseUrls(args[0]).UseStartup<Startup>();


    }
}
