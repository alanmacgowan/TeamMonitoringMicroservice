
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeamMonitoring.EventProcessor.Events;
using RabbitMQ.Client;
using System.Text;
using TeamMonitoring.EventProcessor.Models;
using System;

namespace TeamMonitoring.EventProcessor.Queues.AMQP
{
    public class AMQPEventEmitter : IEventEmitter
    {
        private ILogger<AMQPEventEmitter> logger;

        private ConnectionFactory connectionFactory;

        private QueueOptions queueOptions;

        protected AMQPOptions amqpOptions;

        public AMQPEventEmitter(ILogger<AMQPEventEmitter> logger,
            IOptions<QueueOptions> queueOptions,
            IOptions<AMQPOptions> serviceOptions)
        {
            this.logger = logger;
            this.queueOptions = queueOptions.Value;
            this.amqpOptions = serviceOptions.Value;

            connectionFactory = new ConnectionFactory();

            connectionFactory.UserName = amqpOptions.Username;
            connectionFactory.Password = amqpOptions.Password;
            connectionFactory.VirtualHost = amqpOptions.VirtualHost;
            connectionFactory.HostName = amqpOptions.HostName;
            connectionFactory.Uri = new Uri(amqpOptions.Uri);

            logger.LogInformation($"Emitting events on queue {this.queueOptions.ProximityDetectedEventQueueName}");
        }

        public void EmitProximityDetectedEvent(ProximityDetectedEvent proximityDetectedEvent)
        {
            using (IConnection conn = connectionFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: queueOptions.ProximityDetectedEventQueueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                    string jsonPayload = proximityDetectedEvent.toJson();
                    var body = Encoding.UTF8.GetBytes(jsonPayload);
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueOptions.ProximityDetectedEventQueueName,
                        basicProperties: null,
                        body: body
                    );
                    logger.LogInformation($"Emitted proximity event of {jsonPayload.Length} bytes to queue.");
                }
            }
        }
    }
}