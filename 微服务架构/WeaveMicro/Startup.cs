using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            string contentRoot = AppDomain.CurrentDomain.BaseDirectory;
            string runRoot = AppDomain.CurrentDomain.BaseDirectory;
            IFileProvider fileProvider = new PhysicalFileProvider(Path.Combine(contentRoot, @"doc"));//静态文件存储目录
            IFileProvider runProvider = new PhysicalFileProvider(runRoot);//运行目录

            string reqPath = runRoot.Replace(contentRoot, "").Replace("\\", "/").TrimEnd('/');
            //  app.UseHttpsRedirection();
            //app.UseMiddleware<CorsMiddleware>();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = "/documents"
            }).UseStaticFiles(new StaticFileOptions
            {
                FileProvider = runProvider,
                RequestPath = reqPath
            });
            //app.UseFileServer(new FileServerOptions()//提供文件目录访问形式
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(contentRoot, "wwwroot", "apiHtml")),
            //    RequestPath = "/apiHtml",
            //    EnableDirectoryBrowsing = true//是否启用目录的属性
            //});

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
