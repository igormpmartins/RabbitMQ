using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Enter queue name");
var queueName = Console.ReadLine();

var factory = new ConnectionFactory
{
    HostName = "localhost",
    VirtualHost = "/",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    //Queue must be created by the publisher app (or created manually)
    CreateConsumer(channel);

    channel.Close();
    connection.Close();
}

void CreateConsumer(IModel channel)
{
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, args) =>
    {
        var message = Encoding.UTF8.GetString(args.Body.ToArray());
        Console.WriteLine($"[Queue {queueName}] Message - {message}");
    };

    var consumerTag = channel.BasicConsume(queueName, true, consumer);

    Console.WriteLine("Awaiting, press any key to exit...");
    Console.ReadLine();

    channel.BasicCancel(consumerTag);
}