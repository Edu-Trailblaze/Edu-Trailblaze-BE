using EduTrailblaze.API.Extensions;
using Common.Logging;
using EduTrailblaze.Repositories;
using Serilog;


namespace EduTrailblaze.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            Log.Information("Start API");
            //        Log.Logger = new LoggerConfiguration()
            //.WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
            //.CreateLogger();
            //var log = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration);
            //        builder.Logging.AddSerilog();
            //        builder.Host.UseSerilog((ctx,conf) =>
            //        {
            //            conf.ReadFrom.Configuration(ctx.Configuration);
            //        });
            try
            {
                builder.Host.UseSerilog(SeriLog.Configure);
                builder.Host.AddAppConfiguration();
                builder.Services.AddInfrastructor(builder.Configuration);

                // Kestrel Config (hide in Header Request)
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.AddServerHeader = false;
                });


                var app = builder.Build();

                app.UseInfrastructure();
                // Configure the HTTP request pipeline.
                //if (app.Environment.IsDevelopment())
                //{}
                app.MapControllers();
                app.MapGet("/env", () => app.Environment.EnvironmentName);
                app.MigrateDatabase<EduTrailblazeDbContext>().Run();

            }

            catch (Exception ex)
            {
                string type = ex.GetType().ToString();
                if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;
                Log.Fatal(ex, "Unhandled Exception");
            }
            finally
            {
                Log.Information("Shutting down API");
                Log.CloseAndFlush();
            }

        }
    }

}
