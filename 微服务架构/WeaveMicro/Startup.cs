using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WeaveMicro
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            string contentRoot = Directory.GetCurrentDirectory();
            string runRoot = AppDomain.CurrentDomain.BaseDirectory;
            IFileProvider fileProvider = new PhysicalFileProvider(Path.Combine(contentRoot, "doc"));//静态文件存储目录
            IFileProvider runProvider = new PhysicalFileProvider(runRoot);//运行目录

            string reqPath = runRoot.Replace(contentRoot, "").Replace("\\","/").TrimEnd('/');
            //  app.UseHttpsRedirection();
            app.UseMiddleware<CorsMiddleware>();
            app.UseStaticFiles()
                .UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = fileProvider,
                    RequestPath = "/documents"
                })
            .UseStaticFiles(new StaticFileOptions
            {
                FileProvider = runProvider,
                RequestPath = reqPath
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
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

                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin,X-Requested-With,Content-Type,Accept,Authorization");
                //context.Request.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Cache-Control", "no-cache");

            }

            //context.Request.ContentType = "application/json";
            //Stream inputstream = context.Request.Body;
            //byte[] b = new byte[inputstream.Length];
            //inputstream.Read(b, 0, (int)inputstream.Length);
            //string inputstr = UTF8Encoding.UTF8.GetString(b);
            await _next(context);
        }
    }
}
