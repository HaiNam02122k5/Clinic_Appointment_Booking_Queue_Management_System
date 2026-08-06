using System.Reflection;
using Clinic.Application.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register Mapster Config
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            // Application services are registered via the infrastructure layer.
            return services;
        }
    }
}
