using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace TeamMonitoring.Common.Logging
{
    public static class HostBuilderExtensions
    {

        public static IHostBuilder AddLogging(this IHostBuilder hostBuilder, string indexName)
        {
            hostBuilder.UseSerilog((context, configuration) =>
                             {
                                 configuration.Enrich.FromLogContext()
                                              .Enrich.WithExceptionDetails()
                                              .Enrich.WithMachineName()
                                              .WriteTo.Console()
                                              .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
                                              {
                                                  AutoRegisterTemplate = true,
                                                  IndexFormat = $"{indexName.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                                              })
                                              .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                                              .ReadFrom.Configuration(context.Configuration);
                             });

            return hostBuilder;
        }
    }
}
