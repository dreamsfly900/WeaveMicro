using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using WeaveDoc;
using WeaveMicroClient;
using wRPC;
using wRPCService;

namespace WeaveRemoteService
{
    
    public class RemoteService:IDisposable
    {
        ServiceChannel service ;
        IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("config.json");
        service[] sric;
        MicroClient mc;
        server ser;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name">服务名称</param>
        /// <param name="resttime">响应时间，默认0ms，可能会造成CPU占用高</param>
        public RemoteService(String Name,int resttime=0)
        {
            
            var config= builder.Build();
            Console.WriteLine("成功加载配置文档config.json");
            service = new ServiceChannel(Convert.ToInt32( config["Port"]));
            service.P2Server.resttime= resttime;
            sric = ToolLoad.GetService();
            //foreach (var s in sric)
            //{
            //    if(s.Route.StartsWith("api/Forecast/"))
            //    Console.WriteLine(s.Route);
            //}
            //注册API文档路由
            ApiDocGen.AddRoute(Name, ref sric, service);
            Console.WriteLine("成功加载DLL服务");
            String ss = Newtonsoft.Json.JsonConvert.SerializeObject(sric);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "funconfig.json");
            sw.Write(ss);
            sw.Close();
            Console.WriteLine("更新funconfig.json");
            String mcip = config["Microcenter"];
            if (mcip != "")
            {
                mc = new MicroClient(mcip.Split(':')[0], Convert.ToInt32(mcip.Split(':')[1]));
            }
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
            if(mc != null)
            if (  mc.Connection())
            {
                Console.WriteLine("成功连接到注册中心");
                if(mc!=null)
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
