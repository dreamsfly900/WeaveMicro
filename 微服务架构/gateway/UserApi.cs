using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Weave.Client;

namespace gateway
{
    public static class UserApi
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {

            //endpoints.MapGet("/user/{id}", async context =>
            //{
            //    // Get user logic...
            //});

            endpoints.MapGet("*/*", Proccessor.agent);
            endpoints.MapPost("/API/{code}", Proccessor.agent);
         
        }
     
    }
    public static class Proccessor
    { 
        public async static Task agent(HttpContext context)
        {
          
            // DI  context.RequestServices.GetService();
            // do something NB
            //await 
            if (context.Request.Method == "Get")
            { }
            else
            { 
            }
            httpmode httpmode = new httpmode();
            httpmode.Headers = context.Request.Headers;
            httpmode.From = context.Request.Form;
            httpmode.Query = context.Request.Query;
            string httpstr=  Newtonsoft.Json.JsonConvert.SerializeObject(httpmode);
            
            await context.Response.WriteAsync($"Hello ~, {context.Request.Method}");
            await context.Response.WriteAsync($" ~, {context.Request.Query["name"]}");
        }
         
    }
}
