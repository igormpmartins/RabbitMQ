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
    channel.ExchangeDeclare("ex.headers", "headers", true, false, null);

    channel.QueueDeclare("my.queue1", true, false, false, null);
    channel.QueueDeclare("my.queue2", true, false, false, null);

    channel.QueueBind("my.queue1", "ex.headers", "",
        new Dictionary<string, object>
        {
            { "x-match", "all" },
            { "job", "convert" },
            { "format", "jpeg" },
        });

    channel.QueueBind("my.queue2", "ex.headers", "",
        new Dictionary<string, object>
        {
            { "x-match", "any" },
            { "job", "convert" },
            { "format", "jpeg" },
        });

    var props = channel.CreateBasicProperties();
    props.Headers = new Dictionary<string, object>
        {
            { "job", "convert" },
            { "format", "jpeg" },
        };

    channel.BasicPublish("ex.headers", "", props, Encoding.UTF8.GetBytes("convert jpeg"));

    props = channel.CreateBasicProperties();
    props.Headers = new Dictionary<string, object>
        {
            { "job", "convert" },
            { "format", "bmp" },
        };

    channel.BasicPublish("ex.headers", "", props, Encoding.UTF8.GetBytes("convert bmp"));

    Console.WriteLine("Awaiting...");
    Console.ReadLine();

    //cleaning up for other demos
    channel.QueueDelete("my.queue1");
    channel.QueueDelete("my.queue2");

    channel.ExchangeDelete("ex.headers");

    channel.Close();
    connection.Close();
}
