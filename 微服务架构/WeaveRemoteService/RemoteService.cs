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
            Console.WriteLine("成功加载配置文档config.json");
            service = new ServiceChannel(Convert.ToInt32( config["Port"]));
            sric = ToolLoad.GetService();
            Console.WriteLine("成功加载DLL服务");
            String ss = Newtonsoft.Json.JsonConvert.SerializeObject(sric);
            System.IO.StreamWriter sw = new System.IO.StreamWriter("funconfig.json");
            sw.Write(ss);
            sw.Close();
            Console.WriteLine("更新funconfig.json");
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
            var config = builder.Build();
            service.Start();
            Console.WriteLine("成功绑定开放端口" + config["Port"]);
            if (mc.Connection())
            {
                Console.WriteLine("成功连接到注册中心");

                mc.RegService(ser);
                Console.WriteLine("已经注册service到注册中心");
            }
            else
                throw new Exception("注册中心无法连接");
            Console.WriteLine("当前运行服务名称："+ ser.Name);
            
        }

        public void Dispose()
        {
            
            mc.Stop();
        }

         
    }
}
