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
using System;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using moneymo.core.apiresponsewrapper;

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

        private readonly string SpecificOrigins = "auth-origins";

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
            services.AddCors(options =>
                {
                    options.AddPolicy(SpecificOrigins,
                  builder =>
                  {
                      builder.SetIsOriginAllowed(isOriginAllowed: _=> true).AllowAnyHeader().AllowAnyMethod();
                  });
                   
                });
            services.AddMvc(MvcOptions()).AddJsonOptions(JsonOptions()).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }
            app.UseAPIResponseWrapperMiddleware();
            app.UseCors(SpecificOrigins);
            //app.UseHttpsRedirection();

            app.UseMvc();
        }

        private Action<MvcOptions> MvcOptions() => options =>
        {
            options.Filters.Add(typeof(ModelValidatorFilter));
            options.Filters.Add(new ProducesAttribute("application/json"));
        };
        private Action<MvcJsonOptions> JsonOptions() => options =>
        {
            options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        };
    }
}
