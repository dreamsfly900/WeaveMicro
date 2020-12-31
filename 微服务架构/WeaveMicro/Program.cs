using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.IO;

namespace WeaveMicro
{
    class Program
    {
       static Weave.Server.WeaveP2Server weaveP2Server = new Weave.Server.WeaveP2Server(Weave.Base.WeaveDataTypeEnum.Bytes);
        static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用Weave微服务中心");
            weaveP2Server.weaveReceiveBitEvent += WeaveP2Server_weaveReceiveBitEvent;
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

        private static void WeaveP2Server_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            switch (command)
            {
                case  0x01:
                   //类型1
                    break;
                case 0x02:
                    //类型2
                    break;
                default:
                    Console.WriteLine("输入的有误，请重新输入");
                    break;
            }
        }
    }
}
