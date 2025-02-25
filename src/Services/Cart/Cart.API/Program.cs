
using Cart.Application;
using Cart.Infrastructure;
using Common.Logging;
using Serilog;

namespace Cart.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Log.Information("Start Basket API");

            builder.Host.UseSerilog(SeriLog.Configure);
            builder.Services.AddControllers();

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructor(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
