
using Cart.API.Extensions;
using Cart.Application;
using Cart.Infrastructure;
using Cart.Infrastructure.AutoMapper;
using Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Cart.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Log.Information($"Start {builder.Environment.ApplicationName} API");
            try
            {
                builder.Host.UseSerilog(SeriLog.Configure);
                builder.Services.AddControllers();

                builder.Services.AddApplicationServices();
                builder.Services.AddConfigurationSettings(builder.Configuration);
                builder.Services.AddInfrastructor(builder.Configuration);

                //configure MassTransit
                builder.Services.ConfigureMassTransit();
                builder.Services.AddAutoMapper(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                }); builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();


                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                string type = ex.GetType().ToString();
                if (type.Equals("StopTheHostException", StringComparison.Ordinal))
                {
                    throw;
                }

                Log.Fatal(ex, "Unhandled Exception");

            }
            finally
            {
                Log.Information($"Shutting down {builder.Environment.ApplicationName}");
                Log.CloseAndFlush();
            }
        }
    }
}
