using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using TeamMonitoring.Common.HttpClient;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.ProximityMonitor.Realtime;
using TeamMonitoring.ProximityMonitor.TeamService;
using TeamMonitoring.ProximityMonitor.Processor;
using Prometheus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using System;
using System.Net.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TeamMonitoring.Common.Middleware;
using Microsoft.Extensions.Logging;

namespace TeamMonitoring.ProximityMonitor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials();
            }));

            services.Configure<AMQPOptions>(Configuration.GetSection("amqp"));

            services.AddAMQPConnection();

            services.AddSignalR();

            var teamServiceOptions = Configuration.GetSection("teamservice").Get<TeamServiceOptions>();
            services.AddHttpClientService("TeamAPI", teamServiceOptions.Url);

            services.AddSingleton<EventingBasicConsumer, AMQPEventingConsumer>();
            services.AddSingleton(typeof(IEventSubscriber<>), typeof(EventSubscriber<>));
            services.AddTransient<ITeamServiceClient, HttpTeamServiceClient>();

            services.AddHostedService<ProximityDetectedEventProcessor>();

            services.AddHealthChecks()
                    .AddRabbitMQ(name: "RabbitMQ", failureStatus: HealthStatus.Degraded)
                    .AddUrlGroup(new Uri($"{teamServiceOptions.Url}/health"), HttpMethod.Get, "Teams API", failureStatus: HealthStatus.Degraded);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseCors("CorsPolicy");

            app.UseExceptionHandling(env, logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHub<TeamMonitoringHub>("/hubs/monitoring");
            });
        }
    }
}
