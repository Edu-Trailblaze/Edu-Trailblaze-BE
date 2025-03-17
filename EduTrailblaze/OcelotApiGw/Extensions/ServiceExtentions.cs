using Ocelot.DependencyInjection;

namespace OcelotApiGw.Extensions
{
    public static class ServiceExtentions
    {
        internal static IServiceCollection AddInfrastructor(this IServiceCollection services, IConfiguration configuration)
        {
           
            return services;
        }

        public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOcelot(configuration);
        }
        public static void ConfigureCORS(this IServiceCollection services, IConfiguration configuration)
        {
            var origin = configuration["AllowedOrigins"];
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins(origin)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
                        //.AllowCredentials());
            });
        }

    }
}
