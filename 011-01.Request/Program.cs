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
    Console.WriteLine("<Type 'exit' to finish application>");

    //using default exchange!
    channel.QueueDeclare("requests", true, false, false, null);
    channel.QueueDeclare("responses", true, false, false, null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine($"\nMessage received:\n{message}\n");
        Console.WriteLine("Enter message:");
    };

    channel.BasicConsume("responses", true, consumer);

    while (true)
    {
        Console.WriteLine("Enter message:");

        var message = Console.ReadLine();
        if (message.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            break;

        channel.BasicPublish("", "requests", null, Encoding.UTF8.GetBytes(message));
    }

    channel.QueueDelete("requests");
    channel.QueueDelete("responses");

    channel.Close();
    connection.Close();
}
