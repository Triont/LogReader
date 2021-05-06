using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using WebApplication25.Models;
using WebApplication25.Services;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Proxies;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Serialization;


namespace WebApplication25
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
            services.AddControllersWithViews();
          
            services.AddScoped<HandleLog>();
            services.AddScoped<HandleLogByLine>();
            services.AddMemoryCache();
            services.AddSession();
            services.AddSingleton<IHostedService, HostService>();
            services.AddHostedService<HostedBackground>();

            services.AddDbContext<AppDbContext>(options =>options.UseLazyLoadingProxies().
      UseSqlServer(Configuration.GetConnectionString("Default")));

          
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
           app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

           app.UseRouting();
            
            app.UseAuthorization();
            app.UseSession();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
