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
using wRPC;

namespace gateway
{
    class Program
    {
         
        static void Main(string[] args)
        { 

           //server[] servers= Funconfig.getConfig();
           // server ser= WeightAlgorithm.Get(servers, "abcd/ff");
            Console.WriteLine("Running demo with Kestrel.");

            CreateHostBuilder(args).Build().Run();
            //mainthread loop
            while (true)
            {
                Console.ReadLine();
            }
        }
        public static IWebHostBuilder CreateHostBuilder(string[] args) =>

            WebHost.CreateDefaultBuilder().UseStartup<Startup>();

        
    }
}
