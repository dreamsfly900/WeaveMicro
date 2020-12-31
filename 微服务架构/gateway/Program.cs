using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace gateway
{
    class Program
    {
         
        static void Main(string[] args)
        {
            Console.WriteLine("Running demo with Kestrel.");

            CreateHostBuilder(args).Build().Run();
            //mainthread loop
            while (true)
            {
                Console.ReadLine();
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 });
        
    }
}
