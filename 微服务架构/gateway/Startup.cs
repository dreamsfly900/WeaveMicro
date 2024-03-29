﻿
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//[assembly: OwinStartup(typeof(gateway.Startup))]
namespace gateway
{
    public class Startup
    {
        static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("config.json");

        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }
        public static IConfigurationRoot config;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            config = builder.Build();
            if (Convert.ToBoolean(config["Authentication"]))
            {
                services.AddAuthentication(config["defaultScheme"])
                  .AddJwtBearer(config["defaultScheme"], options =>
                  {
                      options.TokenValidationParameters.ValidateIssuer = false;
                      //  options.TokenValidationParameters.RequireAudience = false;
                      options.Authority = config["IdentityServer"];
                      options.RequireHttpsMetadata = false;

                      options.Audience = config["Audience"];
                  });
            }
        }
        //public void Configuration(IAppBuilder app)
        //{
        //    //ConfigureAuth(app);OAuth2.0
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Proccessor.servers = Funconfig.getConfig();

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //设置不允许的HTTP方法，如TRACE
            app.UseMiddleware<Http405Middleware>(config);
            //添加额外的头部
            app.UseMiddleware<AddHeaderMiddleware>(config);
            // app.UseIdentityServer();
            // app.UseStaticFiles();

            //  app.UseRouting();
            if (Convert.ToBoolean(config["Authentication"]))
            {
                app.UseAuthentication();
            }
            //app.UseAuthorization();
            app.UseMiddleware<CorsMiddleware>();

            app.Run(Proccessor.agent);
            //app.UseEndpoint(endpoints =>
            //{

            //    UserApi.Map(endpoints);
            //    // endpoints.MapRazorPages();
            //});
        }
    }
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {

                context.Response.Headers.Remove("Access-Control-Allow-Origin");
                context.Response.Headers.Remove("Access-Control-Allow-Methods");
                context.Response.Headers.Remove("Access-Control-Allow-Headers");
                context.Response.Headers.Remove("Cache-Control");
                context.Response.Headers.Remove("Access-Control-Expose-Headers");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS,NONE");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Cache-Control", "no-cache");

                context.Response.Headers.Append("Access-Control-Expose-Headers", "Authorization");
                //token为自定义响应头
                context.Response.Headers.Append("Access-Control-Expose-Headers", "token");
            }
            //else
            //{
            //    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS,NONE");
            //    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

            //    context.Response.Headers.Add("Cache-Control", "no-cache");
            //    context.Response.Headers.Add("Access-Control-Expose-Headers", "Authorization");
            //    //token为自定义响应头
            //    context.Response.Headers.Add("Access-Control-Expose-Headers", "token");
            //}

            //context.Request.ContentType = "application/json";
            //Stream inputstream = context.Request.Body;
            //byte[] b = new byte[inputstream.Length];
            //inputstream.Read(b, 0, (int)inputstream.Length);
            //string inputstr = UTF8Encoding.UTF8.GetString(b);
            await _next(context);
        }
    }

    public class Http405Middleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<string> disableMethods;
        public Http405Middleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            disableMethods = config.GetSection("HttpDisableMethods")?.Get<string[]>()?.Select(d => d.ToUpper()) ?? default;
        }

        public async Task Invoke(HttpContext context)
        {
            if (disableMethods != null && disableMethods.Contains(context.Request.Method.ToUpper()))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }
            await _next(context);
        }
    }
    public class AddHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, string> _addHeaders;
        public AddHeaderMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _addHeaders = config.GetSection("AddHeaders")?.Get<Dictionary<string, string>>() ?? default;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_addHeaders != null)
                foreach (var s in _addHeaders)
                    if (!string.IsNullOrWhiteSpace(s.Key))
                        if (string.IsNullOrWhiteSpace(s.Value)) context.Response.Headers.Remove(s.Key);
                        else context.Response.Headers.Add(s.Key, s.Value);
            await _next(context);
        }
    }
}
