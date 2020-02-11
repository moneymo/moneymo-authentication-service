using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moneymo.AuthenticationService.Core.Helpers;
using Moneymo.AuthenticationService.Core.Services;
using Moneymo.AuthenticationService.Data.Models;
using moneymo.core.apiresponsewrapper.Extensions;
using Moneymo.AuthenticationService.Core;

namespace Moneymo.AuthenticationService.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(env.ContentRootPath)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                      .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddTransient<IApiUserService, ApiUserService>();
            services.AddDbContext<MoneymoDbContext>(options => options.UseNpgsql(Configuration.GetValue<string>("ConnectionString")));
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            var authServiceConfiguration = new AuthenticationServiceConfiguration();
            Configuration.GetSection("AuthenticationService").Bind(authServiceConfiguration);

            services.AddSingleton(authServiceConfiguration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAPIResponseWrapperMiddleware();
            if (env.IsDevelopment())
            {
               // app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
               
            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
