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

  
    public static class Proccessor
    {
        public static server[] servers;
        public async static Task agent(HttpContext context)
        {
           
            Dictionary<string, String> servicesDic = new Dictionary<string, String>();
            dynamic contentFromBody = "";
           
            if (servers == null)
            { 
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 999, msg = "非法请求" }));
                return;
            }
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
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 404, msg = "非法请求" }));
                //context.Abort();
                return;
            }
            else
            {
                if (!IsAuthenticated(context, ser.services[0].Authorize))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 0, msg = "非法请求" }));
                   // context.Abort();
                    return;
                }
                
                try
                {
                    DateTime dt = DateTime.Now;
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
                                objs[i] = context.Request.Query[ser.services[0].parameter[i]].ToString();
                            }
                        }
                        else if (ser.services[0].Method == "POST")
                        {
                            if (!context.Request.HasFormContentType)
                            {
                               
                            }
                            else
                            {
                                objs[i] = context.Request.Form[ser.services[0].parameter[i]].ToString();
                            }
                        }
                    }
                    string datastr = Newtonsoft.Json.JsonConvert.SerializeObject(context.Request.Headers);
                    
                    String rl = context.Request.Path.Value.Trim('/');
                    String[] rls = rl.Split('/');
                    rl = "";
                    for (int i = 0; i < rls.Length - 1; i++)
                    {
                        rl += rls[i] + "/";
                    }
                    rl = rl.Substring(0, rl.Length - 1);
                    String retun = CallServer.CallService(ser, rl, rls[rls.Length - 1], objs);
                    //String retun =  clientChannel.Call<String>(rl, rls[rls.Length - 1], objs);
                  
                    await context.Response.WriteAsync($"{retun}");
                    DateTime dt2 = DateTime.Now;
                //    Console.WriteLine((dt2 - dt).TotalMilliseconds);
                    return;
                    //string httpstr = Newtonsoft.Json.JsonConvert.SerializeObject(httpmode);
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync($" ~, {  ex.Message}");
                    return;
                }
                finally
                {
                    
                }

            }



        }

         static bool IsAuthenticated(HttpContext context, bool Authorize)
        {
            if (Convert.ToBoolean(Startup.config["Authentication"]) )
            {
                if (Authorize)
                {
                    if (!context.User.Identity.IsAuthenticated)
                    {
                        // await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 0, msg = "非法请求" }));
                        //  context.Abort();
                        return false;
                    }
                    else
                        return true;
                }
                else
                    return true;
            }else
            return true;
        }
    }
}
