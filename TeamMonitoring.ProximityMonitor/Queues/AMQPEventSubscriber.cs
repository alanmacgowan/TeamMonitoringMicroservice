using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TeamMonitoring.ProximityMonitor.Events;

namespace TeamMonitoring.ProximityMonitor.Queues
{
    public class AMQPEventSubscriber : IEventSubscriber
    {
        public event ProximityDetectedEventReceivedDelegate ProximityDetectedEventReceived;

        protected readonly ILogger _logger;
        protected readonly IConnectionFactory _connectionFactory;
        protected readonly QueueOptions _queueOptions;
        protected readonly EventingBasicConsumer _consumer;
        protected readonly IModel _channel;
        protected string _consumerTag;

        public AMQPEventSubscriber(ILogger<AMQPEventSubscriber> logger,
                                   IOptions<QueueOptions> queueOptions,
                                   IConnectionFactory connectionFactory,
                                   EventingBasicConsumer consumer)
        {
            _logger = logger;
            _queueOptions = queueOptions.Value;
            _connectionFactory = connectionFactory;
            _consumer = consumer;
            _channel = _consumer.Model;

            _logger.LogInformation("Created RabbitMQ event subscriber.");

            Initialize();
        }

        private void Initialize()
        {
            _channel.QueueDeclare(
               queue: _queueOptions.ProximityDetectedEventQueueName,
               durable: false,
               exclusive: false,
               autoDelete: false,
               arguments: null
            );

            _consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                var evt = JsonConvert.DeserializeObject<ProximityDetectedEvent>(msg);
                _logger.LogInformation($"Received incoming event, {body.Length} bytes.");
                if (ProximityDetectedEventReceived != null)
                {
                    ProximityDetectedEventReceived(evt);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        public void Subscribe()
        {
            _consumerTag = _channel.BasicConsume(_queueOptions.ProximityDetectedEventQueueName, false, _consumer);
            _logger.LogInformation($"Subscribed to queue {_queueOptions.ProximityDetectedEventQueueName}, ctag = {_consumerTag}");
        }

        public void Unsubscribe()
        {
            _channel.BasicCancel(_consumerTag);
            _logger.LogInformation($"Stopped subscription on queue {_queueOptions.ProximityDetectedEventQueueName}");
        }
    }
}