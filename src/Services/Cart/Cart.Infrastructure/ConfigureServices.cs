using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Cart.Application.Common.Interfaces;
using Cart.Infrastructure.Repositories;

namespace Cart.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructor(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICartRepository, CartRepository>();
            return services;
        }
        }
}
