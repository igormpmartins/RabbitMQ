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
    channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);
    channel.ExchangeDeclare("ex.direct", "direct", true, false, 
        new Dictionary<string, object>
        {
            { "alternate-exchange", "ex.fanout" }
        });

    channel.QueueDeclare("my.queue1", true, false, false, null);
    channel.QueueDeclare("my.queue2", true, false, false, null);
    channel.QueueDeclare("my.unrouted", true, false, false, null);

    channel.QueueBind("my.queue1", "ex.direct", "image");
    channel.QueueBind("my.queue2", "ex.direct", "video");
    channel.QueueBind("my.unrouted", "ex.fanout", "");

    channel.BasicPublish("ex.direct", "image", null, Encoding.UTF8.GetBytes("Routing key is: image"));
    channel.BasicPublish("ex.direct", "text", null, Encoding.UTF8.GetBytes("Routing key is: text "));

    Console.WriteLine("Awaiting...");
    Console.ReadLine();

    //cleaning up for other demos
    channel.QueueDelete("my.queue1");
    channel.QueueDelete("my.queue2");
    channel.QueueDelete("my.unrouted");

    channel.ExchangeDelete("ex.direct");
    channel.ExchangeDelete("ex.fanout");

    channel.Close();
    connection.Close();
}
