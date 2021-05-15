using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace TeamMonitoring.Common.Queues
{

    public class EventEmitter<T> : IEventEmitter<T> where T : IEvent<T>
    {
        protected readonly string QUEUE_NAME = typeof(T).Name;
        protected readonly ILogger _logger;
        protected readonly IConnectionFactory _connectionFactory;

        public EventEmitter(ILogger<EventEmitter<T>> logger,
                            IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;

            _logger.LogInformation($"Emitting events on queue {QUEUE_NAME}");
        }

        public void EmitEvent(T emmitEvent)
        {
            using (var conn = _connectionFactory.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                    var jsonPayload = emmitEvent.toJson();
                    var body = Encoding.UTF8.GetBytes(jsonPayload);
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: QUEUE_NAME,
                        basicProperties: null,
                        body: body
                    );
                    _logger.LogInformation($"Emitted proximity event of {jsonPayload.Length} bytes to queue.");
                }
            }
        }
    }
}