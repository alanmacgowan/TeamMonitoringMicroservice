
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using TeamMonitoring.EventProcessor.Events;

namespace TeamMonitoring.EventProcessor.Queues
{
    public class AMQPEventEmitter : IEventEmitter
    {
        protected readonly ILogger<AMQPEventEmitter> _logger;
        protected readonly IConnectionFactory _connectionFactory;
        protected readonly QueueOptions _queueOptions;

        public AMQPEventEmitter(ILogger<AMQPEventEmitter> logger,
                                IOptions<QueueOptions> queueOptions,
                                IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _queueOptions = queueOptions.Value;
            _connectionFactory = connectionFactory;

            _logger.LogInformation($"Emitting events on queue {_queueOptions.ProximityDetectedEventQueueName}");
        }

        public void EmitProximityDetectedEvent(ProximityDetectedEvent proximityDetectedEvent)
        {
            using (IConnection conn = _connectionFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _queueOptions.ProximityDetectedEventQueueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                    string jsonPayload = proximityDetectedEvent.toJson();
                    var body = Encoding.UTF8.GetBytes(jsonPayload);
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _queueOptions.ProximityDetectedEventQueueName,
                        basicProperties: null,
                        body: body
                    );
                    _logger.LogInformation($"Emitted proximity event of {jsonPayload.Length} bytes to queue.");
                }
            }
        }
    }
}