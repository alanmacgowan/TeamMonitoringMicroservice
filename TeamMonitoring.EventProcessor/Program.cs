using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.Common.Redis;
using TeamMonitoring.EventProcessor.Events;
using TeamMonitoring.EventProcessor.Location;
using TeamMonitoring.EventProcessor.Location.Redis;

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
                    services.Configure<AMQPOptions>(hostContext.Configuration.GetSection("amqp"));

                    services.AddRedisConnectionMultiplexer(hostContext.Configuration);

                    services.AddAMQPConnection();

                    services.AddSingleton<EventingBasicConsumer, AMQPEventingConsumer>();
                    services.AddSingleton<ILocationCache, RedisLocationCache>();
                    services.AddSingleton(typeof(IEventSubscriber<>), typeof(EventSubscriber<>));
                    services.AddSingleton(typeof(IEventEmitter<>), typeof(EventEmitter<>));

                    services.AddHostedService<MemberLocationEventProcessor>();
                });
    }
}
