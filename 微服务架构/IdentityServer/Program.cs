// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using WeaveVerify;

namespace IdentityServer
{
    public class Program
    {
        static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("config.json");

        public static ConcurrentDictionary<String, IdentityBase> keyValuePairs = GetService();
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer4 - 认证中心";
             
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = builder.Build();
            
            OAuthConfig.UserApi.ApiName =  string.IsNullOrWhiteSpace(config["apiname"]) ? "api1" : config["apiname"];   
            var certificate = new X509Certificate2("server.pfx", config["httpspassword"]);
            return WebHost.CreateDefaultBuilder().UseUrls(config["applicationUrl"])
                    .UseStartup<Startup>().UseKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(options => { options.ServerCertificate = certificate; });
                    })
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
        public static ConcurrentDictionary<String, IdentityBase> GetService()
        {
            ConcurrentDictionary<String, IdentityBase> listservice = new ConcurrentDictionary<String, IdentityBase>();
            String[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            foreach (String file in files)
            {
                try
                {

                    Assembly assembly = Assembly.LoadFrom(file);


                    Type[] ts = assembly.GetTypes();
                    foreach (Type tt in ts)
                    {
                        try
                        {

                            //  MethodInfo[] mis = tt.GetMethods();


                            object obj = assembly.CreateInstance(tt.FullName);
                            if (obj is IdentityBase)
                            {
                                try
                                {
                                    IdentityBase oob = obj as IdentityBase;

                                    listservice.TryAdd(oob.PrjName, oob);
                                }
                                catch (Exception ee)
                                {
                                    Console.WriteLine("GetService--eee-" + ee.Message);
                                }

                            }

                        }
                        catch (Exception ee2)
                        {
                           // Console.WriteLine(tt.FullName + "GetService22---" + ee2.Message);
                        }

                    }
                }
                catch (Exception e)
                { 
                    Console.WriteLine(file+"---GetService---" +e.Message); 
                }
            }
            return listservice;
        }
    }
}