using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
         
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/connect/token")
            {
                if (context.Request.ContentLength != null)
                {
                    context.Request.EnableBuffering();
                    string[] data;
                    using (var mem = new MemoryStream())
                    using (var reader = new StreamReader(mem))
                    {
                        // 
                        await context.Request.Body.CopyToAsync(mem);
                        context.Request.Body.Seek(0, SeekOrigin.Begin);
                        mem.Seek(0, SeekOrigin.Begin);
                        var body = reader.ReadToEnd();
                        data = body.Split("&");
                    }
                    Dictionary<string, String> servicesDic = new Dictionary<string, String>();
                        foreach(string datastr in data)
                        {
                            servicesDic.Add(datastr.Split("=")[0], datastr.Split("=")[1]);
                        }
                      
                        if (!Account.Account.GetLogin(servicesDic, context))
                        {
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 0, msg = "授权失败" }));
                            context.Abort();
                           
                         
                          
                           
                        }
                        //Console.WriteLine(body);

                        // Do something
                   
                   

                }
            }


             await _next(context);
        }
    }

     
}
