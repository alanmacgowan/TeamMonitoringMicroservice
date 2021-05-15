using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TeamMonitoring.EventProcessor.Events;

namespace TeamMonitoring.EventProcessor.Queues
{
    public class AMQPEventSubscriber : IEventSubscriber
    {
        public event MemberLocationRecordedEventReceivedDelegate MemberLocationRecordedEventReceived;

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

            Initialize();
        }

        private void Initialize()
        {
            _channel.QueueDeclare(
                queue: _queueOptions.MemberLocationRecordedEventQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation($"Initialized event subscriber for queue {_queueOptions.MemberLocationRecordedEventQueueName}");

            _consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                var evt = JsonConvert.DeserializeObject<MemberLocationRecordedEvent>(msg);
                _logger.LogInformation($"Received incoming event, {body.Length} bytes.");
                if (MemberLocationRecordedEventReceived != null)
                {
                    MemberLocationRecordedEventReceived(evt);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        public void Subscribe()
        {
            _consumerTag = _channel.BasicConsume(_queueOptions.MemberLocationRecordedEventQueueName, false, _consumer);
            _logger.LogInformation("Subscribed to queue.");
        }

        public void Unsubscribe()
        {
            _channel.BasicCancel(_consumerTag);
            _logger.LogInformation("Unsubscribed from queue.");
        }
    }
}