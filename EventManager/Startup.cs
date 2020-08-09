using System;
using System.IO;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using EventManager.ActionFilters;
using EventManager.Extensions;
using EventManager.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Repository.DataShaping;

namespace EventManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.ThrowExceptions = true;
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureLoggerService();
            services.ConfigureMySqlContext(Configuration);
            services.ConfigureRepositoryManager();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddAutoMapper(typeof(Startup));
            services.ConfigureContentNegotiations();
            services.AddCustomMediaTypes();
            services.ConfigureActionFilter();
            services.AddScoped<EventLinks>();
            services.AddScoped<IDataShaper<EventDTO>, DataShaper<EventDTO>>();
                    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.ConfigureCustomExceptionMiddleware();

            app.UseStaticFiles();

            app.UseCors("CORS Policy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
