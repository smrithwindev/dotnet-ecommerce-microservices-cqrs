using BuildingBlocks.Web.DependencyInjection;
using Microsoft.AspNetCore.Builder; // Ensure this is included for IApplicationBuilder
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Mappings;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Register your infrastructure services here
            //For example: services.AddScoped<IProductRepository, ProductRepository>();

            var fileName = config["MySerilog:FileName"] ?? "default-log";

            SharedServiceContainer.AddSharedServices<ProductDbContext>(
                services,
                config,
                fileName
            );
            //SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]!);

            services.AddScoped<IProductRepository, ProductRepository>();

            //register AutoMapper profiles from the assembly containing ProductMappingProfile
            services.AddAutoMapper(
                cfg => { },
                typeof(ProductMappingProfile).Assembly
            );

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register middleware such as:
            //Global Exception : Handles external errors.
            //Listen to Only Api Gateway : blocks all outsider calls;

            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
