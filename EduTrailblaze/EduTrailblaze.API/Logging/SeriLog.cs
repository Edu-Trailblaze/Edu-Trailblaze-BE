using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace EduTrailblaze.API.Logging
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
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["Elastic:Uri"]))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                    IndexFormat = $"edutrail-{applicationName}-logs-{environmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy.MM}",
                    NumberOfShards = 2,
                    NumberOfReplicas = 1,
                    ModifyConnectionSettings = x => x.BasicAuthentication(context.Configuration["Elastic:Username"], context.Configuration["Elastic:Password"])
                })
                .WriteTo.ApplicationInsights(context.Configuration["ApplicationInsights:InstrumentationKey"],
            TelemetryConverter.Traces)
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
