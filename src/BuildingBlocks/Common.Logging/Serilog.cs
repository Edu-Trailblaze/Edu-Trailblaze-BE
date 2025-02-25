using Microsoft.Extensions.Hosting;
using Serilog;
namespace Common.Logging
{
    public static class SeriLog
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, configuration) =>
        {
            var environmentName = context.HostingEnvironment.EnvironmentName ?? "Development";
            var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");

            configuration
                .WriteTo.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProperty("Environment", environmentName)
                //.WriteTo.File($"Logs/{applicationName}-{environmentName}.log", rollingInterval: RollingInterval.Day)
                .ReadFrom.Configuration(context.Configuration);
            //.Enrich.FromLogContext();

        };

    }
}
