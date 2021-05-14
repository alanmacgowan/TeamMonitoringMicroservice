using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using TeamMonitoring.ProximityMonitor.Events;

namespace TeamMonitoring.ProximityMonitor.Queues
{
    public class RabbitMQEventSubscriber : IEventSubscriber
    {
        public event ProximityDetectedEventReceivedDelegate ProximityDetectedEventReceived;

        private ConnectionFactory connectionFactory;
        protected AMQPOptions amqpOptions;
        private QueueOptions queueOptions;
        private EventingBasicConsumer consumer;
        private IModel channel;
        private string consumerTag;
        private ILogger logger;

        public RabbitMQEventSubscriber(ILogger<RabbitMQEventSubscriber> logger,
            IOptions<QueueOptions> queueOptions,
            IOptions<AMQPOptions> serviceOptions)
        {
            this.queueOptions = queueOptions.Value;
            this.logger = logger;

            this.amqpOptions = serviceOptions.Value;

            connectionFactory = new ConnectionFactory();

            connectionFactory.UserName = amqpOptions.Username;
            connectionFactory.Password = amqpOptions.Password;
            connectionFactory.VirtualHost = amqpOptions.VirtualHost;
            connectionFactory.HostName = amqpOptions.HostName;
            connectionFactory.Uri = new Uri(amqpOptions.Uri);

            var basicConsumer = new EventingBasicConsumer(connectionFactory.CreateConnection().CreateModel());

            this.consumer = basicConsumer;

            this.channel = consumer.Model;

            logger.LogInformation("Created RabbitMQ event subscriber.");
            Initialize();
        }

        private void Initialize()
        {
            channel.QueueDeclare(
               queue: queueOptions.ProximityDetectedEventQueueName,
               durable: false,
               exclusive: false,
               autoDelete: false,
               arguments: null
           );
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                var evt = JsonConvert.DeserializeObject<ProximityDetectedEvent>(msg);
                logger.LogInformation($"Received incoming event, {body.Length} bytes.");
                if (ProximityDetectedEventReceived != null)
                {
                    ProximityDetectedEventReceived(evt);
                }
                channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        public void Subscribe()
        {
            consumerTag = channel.BasicConsume(queueOptions.ProximityDetectedEventQueueName, false, consumer);
            logger.LogInformation($"Subscribed to queue {queueOptions.ProximityDetectedEventQueueName}, ctag = {consumerTag}");
        }

        public void Unsubscribe()
        {
            channel.BasicCancel(consumerTag);
            logger.LogInformation($"Stopped subscription on queue {queueOptions.ProximityDetectedEventQueueName}");
        }
    }
}