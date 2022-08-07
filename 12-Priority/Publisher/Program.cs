using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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

    channel.ExchangeDeclare("ex.fanout", "fanout", true, true);

    var queueArguments = new Dictionary<string, object>
    {
        {"x-max-priority", 2 }
    };

    channel.QueueDeclare("my.queue", true, false, false, queueArguments);
    channel.QueueBind("my.queue", "ex.fanout", "");

    Console.WriteLine("Publisher is ready. Press any key to continue");
    Console.ReadKey();

    SendMessage(channel, 1);
    SendMessage(channel, 1);
    SendMessage(channel, 1);

    SendMessage(channel, 2);
    SendMessage(channel, 2);

    Console.WriteLine("Press any key to exit");
    Console.ReadKey();

    channel.Close();
    connection.Close();
}

void SendMessage(IModel channel, byte priority)
{
    var basicProps = channel.CreateBasicProperties();
    basicProps.Priority = priority;

    var message = $"Message with priority={priority}";
    channel.BasicPublish("ex.fanout", "", basicProps, Encoding.UTF8.GetBytes(message));

    Console.WriteLine($"Sent:{message}");
}