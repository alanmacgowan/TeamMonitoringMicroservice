using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using TeamMonitoring.EventProcessor.Models;

namespace TeamMonitoring.EventProcessor.Queues.AMQP
{
   // public class AMQPEventingConsumer : EventingBasicConsumer
    //{
    //    public AMQPEventingConsumer(ILogger<AMQPEventingConsumer> logger,
    //        IConnectionFactory factory) : base(factory.CreateConnection().CreateModel())
    //    {         
    //    }

    //}
}