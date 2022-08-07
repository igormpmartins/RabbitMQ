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
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine($"\nRequest received:\n{message}\n");

        var response = $"Reply for {message}";

        channel.BasicPublish("", "responses", null, Encoding.UTF8.GetBytes(response));
    };

    channel.BasicConsume("requests", true, consumer);

    Console.WriteLine("Press any key to exit");
    Console.ReadKey();

    channel.Close();
    connection.Close();
}
