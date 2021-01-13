using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using testdll2;
using wRPC;
using wRPCService;

namespace Test
{
   
    class Program
    {
       static wRPCclient.ClientChannel clientChannel = new wRPCclient.ClientChannel("127.0.0.1", 10098);
        static  void Main(string[] args)
        {
           
            service[] sric = ToolLoad.GetService();
            ServiceChannel service = new ServiceChannel(10098);
            service.Start();
            while (true)
            {
                System.Threading.Thread.Sleep(10);
             //   bb();
                Console.ReadLine();
            }

            
            Console.WriteLine("H6ello World!");
        }

       async static void bb()
        {
         
            String retun = await clientChannel.Call<String>("abcd", "ff", new object[] { new { name = "gghhss", age = 0 }, new { name = "gghhss", age = 0 } });
            Console.WriteLine("ceshi:"+retun);
             
        }
    }
    
} 
