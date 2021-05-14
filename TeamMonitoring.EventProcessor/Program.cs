using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<QueueOptions>(hostContext.Configuration.GetSection("QueueOptions"));
                    services.Configure<AMQPOptions>(hostContext.Configuration.GetSection("amqp"));

                    services.AddRedisConnectionMultiplexer(hostContext.Configuration);

                    services.AddSingleton(typeof(ILocationCache), typeof(RedisLocationCache));
                    services.AddSingleton(typeof(IEventSubscriber), typeof(AMQPEventSubscriber));
                    services.AddSingleton(typeof(IEventEmitter), typeof(AMQPEventEmitter));

                    services.AddHostedService<MemberLocationEventProcessor>();
                });
    }
}
