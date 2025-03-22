using Cart.Application.Common.Interfaces;
using Cart.Infrastructure.Repositories;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Cart.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructor(this IServiceCollection services, IConfiguration configuration)
        {

            var redisConfiguration = new ConfigurationOptions
            {
                EndPoints = { $"{configuration["RedisConfig:Host"]}:{configuration["RedisConfig:Port"]}" },
                Password = configuration["RedisConfig:Password"],
                Ssl = bool.Parse(configuration["RedisConfig:Ssl"]),
                AbortOnConnectFail = bool.Parse(configuration["RedisConfig:AbortOnConnectFail"]),
                ConnectRetry = 5,
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                KeepAlive = 180

            };
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = $"{configuration["RedisConfig:Host"]}:{configuration["RedisConfig:Port"]}";
            //    options.ConfigurationOptions = redisConfiguration;
            //});
            //services.AddSingleton(redisConfiguration);
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
             // var configuration = sp.GetRequiredService<ConfigurationOptions>();
                return ConnectionMultiplexer.Connect(redisConfiguration);
            });
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<ISerializeService, SerializeService>();
            services.AddScoped<ICartRepository, CartRepository>();
            return services;
        }
    }
    public class RedisConfig
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Password { get; set; }
        public string Ssl { get; set; }
        public string AbortOnConnectFail { get; set; }
    }
}
