using RabbitMQ.Client;
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
    channel.ExchangeDeclare("ex.direct", "direct", true, false, null);

    channel.QueueDeclare("my.infos", true, false, false, null);
    channel.QueueDeclare("my.warnings", true, false, false, null);
    channel.QueueDeclare("my.errors", true, false, false, null);

    channel.QueueBind("my.infos", "ex.direct", "info");
    channel.QueueBind("my.warnings", "ex.direct", "warning");
    channel.QueueBind("my.errors", "ex.direct", "error");

    channel.BasicPublish("ex.direct", "info", null, 
        Encoding.UTF8.GetBytes("Direct message - info"));

    channel.BasicPublish("ex.direct", "warning", null, 
        Encoding.UTF8.GetBytes("Direct message - warning"));

    channel.BasicPublish("ex.direct", "error", null, 
        Encoding.UTF8.GetBytes("Direct message - error"));

    Console.WriteLine("Awaiting...");
    Console.ReadLine();

    //cleaning up for other demos
    channel.QueueDelete("my.infos");
    channel.QueueDelete("my.warnings");
    channel.QueueDelete("my.errors");

    channel.ExchangeDelete("ex.direct");

    channel.Close();
    connection.Close();
}
