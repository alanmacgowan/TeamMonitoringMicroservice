using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace TeamMonitoring.Common.Queues
{
    public static class AMQPExtensions
    {

        public static IServiceCollection AddAMQPConnection(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionFactory, ConnectionFactory>(provider =>
            {
                var connectionFactory = new ConnectionFactory();
                var amqpOptions = provider.GetService<IOptions<AMQPOptions>>().Value;

                connectionFactory.UserName = amqpOptions.Username;
                connectionFactory.Password = amqpOptions.Password;
                connectionFactory.VirtualHost = amqpOptions.VirtualHost;
                connectionFactory.Uri = new Uri(amqpOptions.Uri);

                return connectionFactory;
            });

            return services;
        }
    }
}
