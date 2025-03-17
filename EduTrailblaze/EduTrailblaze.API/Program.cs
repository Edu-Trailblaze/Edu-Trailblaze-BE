using Common.Logging;
using EduTrailblaze.API.Extensions;
using EduTrailblaze.Repositories;
using Hangfire;
using Hangfire.SqlServer;
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

                builder.Services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

                builder.Services.AddHangfireServer();

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

                app.UseHangfireDashboard();
                app.UseStaticFiles();
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
