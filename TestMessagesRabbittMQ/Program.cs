using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TestRabbit
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            const string queueName = "testqueue";

            try
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = "192.168.56.1",
                    UserName = "test",
                    Password = "test",
                    Port = 5672,
                };

                using (var rabbitConnection = connectionFactory.CreateConnection())
                {
                    using (var channel = rabbitConnection.CreateModel())
                    {
                        // Declaring a queue is idempotent 
                        channel.QueueDeclare(
                            queue: queueName,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        while (true)
                        {
                            string body = $"A nice random message: {DateTime.Now.Ticks}";
                            channel.BasicPublish(
                                exchange: string.Empty,
                                routingKey: queueName,
                                basicProperties: null,
                                body: Encoding.UTF8.GetBytes(body));

                            Console.WriteLine("Message sent");
                            await Task.Delay(500);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("End");
            Console.Read();
        }
    }
}