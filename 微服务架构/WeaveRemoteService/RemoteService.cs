using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using WeaveMicroClient;
using wRPC;
using wRPCService;

namespace WeaveRemoteService
{
    public class RemoteService:IDisposable
    {
        ServiceChannel service ;
        IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json");
        service[] sric;
        MicroClient mc;
        server ser;
        public RemoteService(String Name)
        {
            var config= builder.Build();
            service = new ServiceChannel(Convert.ToInt32( config["Port"]));
            sric = ToolLoad.GetService();

            String ss = Newtonsoft.Json.JsonConvert.SerializeObject(sric);
            System.IO.StreamWriter sw = new System.IO.StreamWriter("funconfig.json");
            sw.Write(ss);
            sw.Close();
            String mcip = config["Microcenter"];
             mc = new MicroClient(mcip.Split(':')[0], Convert.ToInt32(mcip.Split(':')[1]));
             ser = new server();
            ser.Name = Name;
            ser.services = sric;
            ser.IP = config["ServerIP"] ;
            ser.Port = Convert.ToInt32(config["Port"]);
            
        }

        public void Start()
        {
            service.Start();
            if (mc.Connection())
            {
               
                mc.RegService(ser);
            }
            else
                throw new Exception("注册中心无法连接");
        }

        public void Dispose()
        {
            
            mc.Stop();
        }

         
    }
}
