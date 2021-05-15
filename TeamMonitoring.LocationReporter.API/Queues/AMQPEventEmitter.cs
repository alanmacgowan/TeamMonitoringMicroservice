using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.LocationReporter.API.Events;

namespace TeamMonitoring.LocationReporter.API.Queues
{

    public class AMQPEventEmitter : IEventEmitter
    {
        protected readonly ILogger _logger;
        protected readonly IConnectionFactory _connectionFactory;
        protected readonly QueueOptions _queueOptions;

        public AMQPEventEmitter(ILogger<AMQPEventEmitter> logger,
                                IOptions<QueueOptions> queueOptions,
                                IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _queueOptions = queueOptions.Value;
            _connectionFactory = connectionFactory;

            _logger.LogInformation("AMQP Event Emitter configured with URI {0}", _connectionFactory.Uri);
        }

        public void EmitLocationRecordedEvent(MemberLocationRecordedEvent locationRecordedEvent)
        {
            using (IConnection conn = _connectionFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _queueOptions.MemberLocationRecordedEventQueueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                    string jsonPayload = locationRecordedEvent.toJson();
                    var body = Encoding.UTF8.GetBytes(jsonPayload);
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _queueOptions.MemberLocationRecordedEventQueueName,
                        basicProperties: null,
                        body: body
                    );
                }
            }
        }
    }
}