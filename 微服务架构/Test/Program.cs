using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;
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
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint MM_BeginPeriod(uint uMilliseconds);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint MM_EndPeriod(uint uMilliseconds);
        static  void Main(string[] args)
        {

             
            
            RemoteService remoteService = new RemoteService("TEST");
            remoteService.Start();
            String retun = clientChannel.Call<object>("big/api/page", "InsertConfig", "{\r\n  \"Id\": 0,\r\n  \"PageName\": \"11\",\r\n  \"Width\": \"1920px\",\r\n  \"Height\": \"1080px\",\r\n  \"Config\": \"[]\",\r\n  \"UserId\": 15,\r\n  \"Createtime\": \"\",\r\n  \"Pushrate\": \"\",\r\n  \"Image\": \"\"\r\n}\r\n");
          
           
            // MM_BeginPeriod(1);//设置休眠精度
                               //while (true)
                               //{
                               //    System.Threading.Thread.Sleep(10);
                               //    DateTime dt = DateTime.Now;
                               //    bb();

            //    DateTime dt2 = DateTime.Now;
            //    Console.WriteLine("Main:" + (dt2 - dt).TotalMilliseconds);
            //}
          // retun = clientChannel.Call<String>("api/abcd", "ff");
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
           
            //   String retun =  clientChannel.Call<String>("abcd", "ff",  new { name = "gghhss", age = 0 } );
          //  Console.WriteLine("ceshi:"+retun);
             
        }
    }
    
} 
