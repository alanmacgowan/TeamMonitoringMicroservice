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

            services.Configure<AMQPOptions>(Configuration.GetSection("amqp"));

            services.AddAMQPConnection();

            var teamServiceOptions = Configuration.GetSection("teamservice").Get<TeamServiceOptions>();
            services.AddHttpClientService("TeamAPI", teamServiceOptions.Url);

            services.AddSingleton<EventingBasicConsumer, AMQPEventingConsumer>();
            services.AddSingleton(typeof(IEventSubscriber<>), typeof(EventSubscriber<>));
            services.AddTransient<ITeamServiceClient, HttpTeamServiceClient>();

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

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TeamMonitoringHub>("/hubs/monitoring");
            });
        }
    }
}
