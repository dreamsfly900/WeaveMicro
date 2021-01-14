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
        public RemoteService()
        {
            var config= builder.Build();
            service = new ServiceChannel(Convert.ToInt32( config["Port"]));
            sric = ToolLoad.GetService();

            //String ss=Newtonsoft.Json.JsonConvert.SerializeObject(sric);
            //System.IO.StreamWriter sw = new System.IO.StreamWriter("config.json");
            //sw.Write(ss);
            //sw.Close();
            String mcip = config["Microcenter"];
             mc = new MicroClient(mcip.Split(':')[0], Convert.ToInt32(mcip.Split(':')[1]));
         
           
        }

        public void Start()
        {
            service.Start();
            if (mc.Connection())
            {
                server ser = new server();
                ser.services = sric;
                ser.IP = "127.0.0.1";
                ser.Port = 10098;
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
