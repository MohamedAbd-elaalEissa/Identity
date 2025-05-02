using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.RabbitMQ
{
    public class RegisterPublisher
    {
        public async Task PublishRegisterDataAsync(string userName, string email, string phoneNumber, bool isTeacher)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // Only hostname
                Port = 5672              // AMQP default port
            }; // RabbitMQ server  
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            var userData = new { UserName = userName, Email = email, PhoneNumber = phoneNumber };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userData));
            if (isTeacher == true)
            {
                await channel.QueueDeclareAsync(queue: "teacherQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                // Fix: Specify the type argument explicitly for BasicPublishAsync  
                await channel.BasicPublishAsync<BasicProperties>(exchange: "",
                                     routingKey: "teacherQueue",
                                     mandatory: false,
                                     basicProperties: new BasicProperties(),
                                     body: body);
            }
            else
            {
                await channel.QueueDeclareAsync(queue: "studentQueue",
                                                 durable: false,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                // Fix: Specify the type argument explicitly for BasicPublishAsync  
                await channel.BasicPublishAsync<BasicProperties>(exchange: "",
                                     routingKey: "studentQueue",
                                     mandatory: false,
                                     basicProperties: new BasicProperties(),
                                     body: body);
            }

        }
    }
}
