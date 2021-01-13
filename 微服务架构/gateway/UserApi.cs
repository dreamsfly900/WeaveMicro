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
using Newtonsoft.Json;

namespace gateway
{

    public static class Funconfig
    {
       static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("funconfig.json");



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
            if (Convert.ToBoolean(Startup.config["Authentication"]))
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 0, msg = "非法请求" }));
                  //  context.Abort();
                     return;
                }

            }
            Dictionary<string, String> servicesDic = new Dictionary<string, String>();
            dynamic contentFromBody = "";
            if (context.Request.ContentLength != null)
            {

                var body = "";
                context.Request.EnableBuffering();
                using (var mem = new MemoryStream())
                using (var reader = new StreamReader(mem))
                {
                    // 
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    await context.Request.Body.CopyToAsync(mem);
                    mem.Seek(0, SeekOrigin.Begin);
                    body = reader.ReadToEnd();

                }
                if (context.Request.ContentType == "application/json")
                {

                    contentFromBody = body;

                }
                else if (context.Request.ContentType == "application/x-www-form-urlencoded")
                {
                    contentFromBody = body.Split("&");

                    foreach (string datastr in contentFromBody)
                    {
                        servicesDic.Add(datastr.Split("=")[0], datastr.Split("=")[1]);
                    }
                }

            }
            server ser =await WeightAlgorithm.Get(servers, context.Request.Path.Value.Trim('/'));
            if (ser == null)
            {
                await context.Response.WriteAsync($" ~, {404}");
                //context.Abort();
                //return;
            }
            else
            {
                wRPCclient.ClientChannel clientChannel = null;
                try
                {
                    //ser.services[0].parameter
                    object[] objs = new object[ser.services[0].parameter.Length];
                    for (int i = 0; i < objs.Length; i++)
                    {

                        if (ser.services[0].Method == "NONE")
                        {
                            if (context.Request.ContentType == "application/json")
                            {
                                objs[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(contentFromBody);
                            }
                            else if (context.Request.ContentType == "application/x-www-form-urlencoded")
                            {
                                objs[i] = servicesDic[ser.services[0].parameter[i]];
                            }
                        }
                        else if (ser.services[0].Method == "GET")
                        {
                            if (context.Request.Query == null)
                            {

                            }
                            else
                            {
                                objs[i] = context.Request.Query[ser.services[0].parameter[i]];
                            }
                        }
                        else if (ser.services[0].Method == "POST")
                        {
                            if (!context.Request.HasFormContentType)
                            {
                               
                            }
                            else
                            {
                                objs[i] = context.Request.Form[ser.services[0].parameter[i]];
                            }
                        }
                    }
                    string datastr = Newtonsoft.Json.JsonConvert.SerializeObject(context.Request.Headers);
                    clientChannel = new wRPCclient.ClientChannel(ser.IP, ser.Port);
                    clientChannel.Headers = context.Request.Headers;
                    String rl = context.Request.Path.Value.Trim('/');
                    String[] rls = rl.Split('/');
                    rl = "";
                    for (int i = 0; i < rls.Length - 1; i++)
                    {
                        rl += rls[i] + "/";
                    }
                    rl = rl.Substring(0, rl.Length - 1);
                    String retun = await clientChannel.Call<String>(rl, rls[rls.Length - 1], objs);

                    await context.Response.WriteAsync($"{retun}");
                    //string httpstr = Newtonsoft.Json.JsonConvert.SerializeObject(httpmode);
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync($" ~, {  ex.Message}");
                }
                finally
                {
                    if (clientChannel != null)
                        clientChannel.Dispose();
                }

            }



        }
        
    }
}
