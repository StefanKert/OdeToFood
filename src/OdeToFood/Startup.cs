using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OdeToFood.Entities;
using OdeToFood.Services;

namespace OdeToFood
{
    public class Startup
    {
        public Startup() {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<OdeToFoodDbContext>(options => options.UseSqlServer(Configuration["database:connection"]));
            
            services.AddSingleton(provider => Configuration);
            services.AddSingleton<IGreeter, Greeter>();
            services.AddScoped<IRestaurantData, SqlRestaurantData>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment environment, IGreeter greeter) {
            app.UseIISPlatformHandler();

            if (environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                //TODO: Add pretty error page for user
            }

            app.UseRuntimeInfoPage("/info");

            app.UseFileServer();

            app.UseMvc(ConfigureRoutes);

            app.Run(async context => {
                await context.Response.WriteAsync(greeter.GetGreeting());
            });
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            // /Home/Index

            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
