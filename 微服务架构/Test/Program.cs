using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using testdll2;
using WeaveMicroClient;
using WeaveRemoteService;
using wRPC;
using wRPCService;

namespace Test
{
   
    class Program
    {
       static wRPCclient.ClientChannel clientChannel = new wRPCclient.ClientChannel("127.0.0.1", 10098);
        static  void Main(string[] args)
        {
            RemoteService remoteService = new RemoteService("TEST");
            remoteService.Start();
 
            while (true)
            {
                System.Threading.Thread.Sleep(10);
             //   bb();
               string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "exit":
                        break;
                    default:
                        continue;
                }
            }
           // mc.Stop();

           // Console.WriteLine("H6ello World!");
        }

        static void bb()
        {
         
            String retun =  clientChannel.Call<String>("abcd", "ff",  new { name = "gghhss", age = 0 } );
            Console.WriteLine("ceshi:"+retun);
             
        }
    }
    
} 
