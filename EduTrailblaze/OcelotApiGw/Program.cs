using Common.Logging;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;
namespace OcelotApiGw
{
    public class Program
    {
        public static  void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Log.Information($"Start {builder.Environment.ApplicationName} API");

            builder.Host.UseSerilog(SeriLog.Configure);

            try { 
            builder.Host.AddAppConfiguration();
            builder.Services.AddInfrastructor(builder.Configuration);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureOcelot(builder.Configuration);
                var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
           app.UseOcelot().Wait();
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
