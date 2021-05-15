using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.Common.Redis;
using TeamMonitoring.EventProcessor.Events;
using TeamMonitoring.EventProcessor.Location;
using TeamMonitoring.EventProcessor.Location.Redis;
using TeamMonitoring.EventProcessor.Queues;

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

                    services.AddAMQPConnection();

                    services.AddSingleton<ILocationCache, RedisLocationCache>();
                    services.AddSingleton<IEventSubscriber, AMQPEventSubscriber>();
                    services.AddSingleton<IEventEmitter, AMQPEventEmitter>();

                    services.AddHostedService<MemberLocationEventProcessor>();
                });
    }
}
