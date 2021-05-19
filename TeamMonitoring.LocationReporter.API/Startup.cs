using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using TeamMonitoring.Common.HttpClient;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.LocationReporter.API.Models;
using TeamMonitoring.LocationReporter.API.Services;

namespace TeamMonitoring.LocationReporter.API
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamMonitoring.LocationReporter.API", Version = "v1" });
            });


            services.Configure<AMQPOptions>(Configuration.GetSection("amqp"));

            services.AddAMQPConnection();

            var teamServiceOptions = Configuration.GetSection("teamservice").Get<TeamServiceOptions>();
            services.AddHttpClientService("TeamAPI", teamServiceOptions.Url);

            services.AddSingleton(typeof(IEventPublisher<>), typeof(EventPublisher<>));
            services.AddSingleton<ICommandEventConverter, CommandEventConverter>();
            services.AddSingleton<ITeamServiceClient, HttpTeamServiceClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamMonitoring.LocationReporter.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMetricServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHttpMetrics();

        }
    }
}
