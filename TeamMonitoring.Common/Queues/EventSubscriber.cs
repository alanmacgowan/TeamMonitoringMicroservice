using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace TeamMonitoring.Common.Queues
{
    public class EventSubscriber<T> : IEventSubscriber<T>
    {
        public event Delegate<T> EventReceived;

        protected readonly string QUEUE_NAME = typeof(T).Name;
        protected readonly ILogger _logger;
        protected readonly IConnectionFactory _connectionFactory;
        protected readonly EventingBasicConsumer _consumer;
        protected readonly IModel _channel;
        protected string _consumerTag;

        public EventSubscriber(ILogger<EventSubscriber<T>> logger,
                               IConnectionFactory connectionFactory,
                               EventingBasicConsumer consumer)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _consumer = consumer;
            _channel = _consumer.Model;

            Initialize();
        }

        private void Initialize()
        {
            _channel.QueueDeclare(
                queue: QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation($"Initialized event subscriber for queue {QUEUE_NAME}");

            _consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                var evt = JsonConvert.DeserializeObject<T>(msg);
                _logger.LogInformation($"Received incoming event, {body.Length} bytes.");
                if (EventReceived != null)
                {
                    EventReceived(evt);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        public void Subscribe()
        {
            _consumerTag = _channel.BasicConsume(QUEUE_NAME, false, _consumer);
            _logger.LogInformation("Subscribed to queue.");
        }

        public void Unsubscribe()
        {
            _channel.BasicCancel(_consumerTag);
            _logger.LogInformation("Unsubscribed from queue.");
        }
    }

}