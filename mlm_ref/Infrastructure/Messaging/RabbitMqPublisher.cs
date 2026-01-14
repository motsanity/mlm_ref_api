using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace mlm_ref.Infrastructure.Messaging
{       
public class RabbitMqPublisher
    {
        private readonly RabbitMqConfig _config;

        public RabbitMqPublisher(IOptions<RabbitMqConfig> config)
        {
            _config = config.Value;
        }

        public async Task <bool> PublishAsync(string queue, string action, string table, object payload)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _config.HostName,
                    UserName = _config.UserName,
                    Password = _config.Password,
                    Port = _config.Port
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var message = new
                    {
                        Table = table,
                        Action = action,
                        Data = payload
                    };

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                    channel.BasicPublish(exchange: "",
                                         routingKey: queue,
                                         basicProperties: null,
                                         body: body);
                }

                await Task.CompletedTask;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}