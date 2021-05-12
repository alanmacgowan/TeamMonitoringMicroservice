using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMonitoring.EventProcessor.Events;
using TeamMonitoring.EventProcessor.Location;
using TeamMonitoring.EventProcessor.Location.Redis;
using TeamMonitoring.EventProcessor.Models;
using TeamMonitoring.EventProcessor.Queues;
using TeamMonitoring.EventProcessor.Queues.AMQP;

namespace TeamMonitoring.EventProcessor
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamMonitoring.EventProcessor", Version = "v1" });
            });

            services.Configure<QueueOptions>(Configuration.GetSection("QueueOptions"));
            services.Configure<AMQPOptions>(Configuration.GetSection("amqp"));

            services.AddRedisConnectionMultiplexer(Configuration);

            //services.AddTransient(typeof(EventingBasicConsumer), typeof(AMQPEventingConsumer));

            services.AddSingleton(typeof(ILocationCache), typeof(RedisLocationCache));

            services.AddSingleton(typeof(IEventSubscriber), typeof(AMQPEventSubscriber));
            services.AddSingleton(typeof(IEventEmitter), typeof(AMQPEventEmitter));
            services.AddSingleton(typeof(IEventProcessor), typeof(MemberLocationEventProcessor));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEventProcessor eventProcessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamMonitoring.EventProcessor v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            eventProcessor.Start();
        }
    }
}
