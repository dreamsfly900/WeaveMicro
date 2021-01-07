
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace gateway
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
            //services.AddControllers();
            //services.AddRazorPages();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            // app.UseStaticFiles();

            //  app.UseRouting();

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

                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
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
