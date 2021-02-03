// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;

namespace IdentityServer
{
    public class Program
    {
        static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json");
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer4";

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = builder.Build();
            return WebHost.CreateDefaultBuilder().UseUrls(config["applicationUrl"])
                    .UseStartup<Startup>()
                    .UseSerilog((context, configuration) =>
                    {
                        configuration
                            .MinimumLevel.Debug()
                        //    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        //    .MinimumLevel.Override("System", LogEventLevel.Warning)
                        //    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                        //    .Enrich.FromLogContext()
                            .WriteTo.File(@"identityserver4_log.txt")
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
                    });
        }
    }
}