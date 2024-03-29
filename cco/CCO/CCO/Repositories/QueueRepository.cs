﻿using CCO.Entities;
using RabbitMQ.Client;
using System.Text;

namespace CCO.Repositories
{
    public class QueueRepository
    {
        public void Publish(QueueSource queue, string queueName, string message)
        {
            using var connection = CreateConnection(queue);
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }

        private static IConnection CreateConnection(QueueSource queue)
        {
            string connectionString = $"amqp://{queue.Username}:{queue.Password}@{queue.Url}";

            var factory = new ConnectionFactory{ Uri = new Uri(connectionString) };

            return factory.CreateConnection();
        }
    }
}
