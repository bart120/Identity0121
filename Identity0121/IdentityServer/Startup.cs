using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Demo;
using IdentityServer.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
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
            var migrationAssemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            services.AddControllersWithViews();

            //validation user AD
            //services.AddScoped<IProfileService, ADProfileService>();

            var builder = services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/login";
                options.UserInteraction.LogoutUrl = "/logout";

            })

                /*.AddInMemoryApiScopes(DemoClientCredentials.ApiScopes)
                .AddInMemoryClients(DemoClientCredentials.Clients);*/
                /*.AddInMemoryIdentityResources(DemoCode.IdentityResources)
                .AddInMemoryApiScopes(DemoCode.ApiScopes)
                .AddInMemoryClients(DemoCode.Clients)*/
                .AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(s =>
                {
                    s.DefaultSchema = "configuration";
                    s.ConfigureDbContext = db => db.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"),
                        sql => sql.MigrationsAssembly(migrationAssemblyName));
                })
                .AddOperationalStore(s =>
                {
                    s.DefaultSchema = "operational";
                    s.ConfigureDbContext = db => db.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"),
                         sql => sql.MigrationsAssembly(migrationAssemblyName));
                });

            builder.AddDeveloperSigningCredential();//only dev

            services.AddAuthentication();

            //.AddTestUsers(new List<IdentityServer4.Test.TestUser> { new IdentityServer4.Test.TestUser {  } });
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
