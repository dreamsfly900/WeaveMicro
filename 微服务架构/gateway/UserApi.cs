using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Weave.Client;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using wRPC;

namespace gateway
{

    public static class Funconfig
    {
       static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json");



        public static server[] getConfig()
        {
            server[] serverlist = new server[0];
            var config=   builder.Build();
            serverlist= config.GetSection("server").Get<server[]>();
            foreach (server ser in serverlist)
            {
                foreach (service serice in ser.services)
                {
                    if(!ser.servicesDic.ContainsKey(serice.Route))
                    ser.servicesDic.GetOrAdd(serice.Route, serice);
                }
            }
            return serverlist;
        }
    }
    public static class Proccessor
    {
        public static server[] servers;
        public async static Task agent(HttpContext context)
        {
            if (context.Request.ContentLength == null)
                return;
            // DI  context.RequestServices.GetService();
            // do something NB
            //await 
            //string directoryPath= System.AppDomain.CurrentDomain.BaseDirectory + "www");
            // if (!Directory.Exists(directoryPath))
            // {
            //     Directory.CreateDirectory(directoryPath);
            // }

            // 文件路径
            
                var reader = new StreamReader(context.Request.Body);
                var contentFromBody = reader.ReadToEnd();
            //Stream inputstream = context.Request.Body;
            //byte[] b = new byte[context.Request.Body.Length];
            //inputstream.Read(b, 0, (int)inputstream.Length);
            //string inputstr = UTF8Encoding.UTF8.GetString(b);
            // httpmode httpmode = new httpmode();
            //if (context.Request.ContentType == "application/json")
            //{
            //    string inputstr =contentFromBody.ToString();
            //}

            server ser = WeightAlgorithm.Get(servers, context.Request.Path.Value.Trim('/'), context.Request.Method);
            if (ser == null)
            {
                await context.Response.WriteAsync($" ~, {404}");
            }
            else
            {
                try
                {
                    //ser.services[0].parameter
                    object[] objs = new object[ser.services[0].parameter.Length];

                    wRPCclient.ClientChannel clientChannel = new wRPCclient.ClientChannel(ser.IP, ser.Port);
                    String retun = await clientChannel.Call<String>("abcd", "ff", context, "gbvas");
                    //string httpstr = Newtonsoft.Json.JsonConvert.SerializeObject(httpmode);
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync($" ~, {ex.Message}");
                }

            }

            
           
        }
        
    }
}
