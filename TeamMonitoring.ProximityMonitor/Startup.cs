using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMonitoring.ProximityMonitor.Events;
using TeamMonitoring.ProximityMonitor.Queues;
using TeamMonitoring.ProximityMonitor.Realtime;
using TeamMonitoring.ProximityMonitor.TeamService;

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

            services.AddSignalR();

            services.Configure<QueueOptions>(Configuration.GetSection("QueueOptions"));
            //services.Configure<PubnubOptions>(Configuration.GetSection("PubnubOptions"));
            services.Configure<TeamServiceOptions>(Configuration.GetSection("teamservice"));
            services.Configure<AMQPOptions>(Configuration.GetSection("amqp"));

            services.AddSingleton(typeof(IEventSubscriber), typeof(RabbitMQEventSubscriber));
            services.AddTransient(typeof(ITeamServiceClient), typeof(HttpTeamServiceClient));

            services.AddHostedService<ProximityDetectedEventProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TeamMonitoringHub>("/hubs/monitoring");
            });
        }
    }
}
