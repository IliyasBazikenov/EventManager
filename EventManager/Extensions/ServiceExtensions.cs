using Contracts;
using Entities;
using EventManager.ActionFilters;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using System.Linq;

namespace EventManager.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CORS Policy",
                    builder => builder.AllowAnyOrigin()
                    .WithMethods("POST", "GET")
                    .WithHeaders("accept", "content-type"));
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["mysqlconnection:connectionString"];
            services.AddDbContextPool<RepositoryContext>(o => o.UseMySql(connectionString,
                b => b.MigrationsAssembly("EventManager")));
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureContentNegotiations(this IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson()
            .AddXmlDataContractSerializerFormatters()
            .AddCustomCSVFormatter();
        }

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
        {
            return builder.AddMvcOptions(config =>
            config.OutputFormatters.Add(new CsvOutputFormatter()));
        }

        public static void ConfigureActionFilter(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ValidateAccountExistsAttribute>();
            services.AddScoped<ValidateEventExistsAttribute>();
            services.AddScoped<ValidateMediaTypeAttribute>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter
                    .SupportedMediaTypes
                    .Add("application/vnd.iliyas.hateoas+json");
                }

                var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter
                    .SupportedMediaTypes
                    .Add("application/vnd.iliyas.hateoas+xml");
                }
            });
        }
    }
}
