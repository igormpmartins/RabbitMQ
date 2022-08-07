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
    channel.ExchangeDeclare("ex.topic", "topic", true, false, null);

    channel.QueueDeclare("my.queue1", true, false, false, null);
    channel.QueueDeclare("my.queue2", true, false, false, null);
    channel.QueueDeclare("my.queue3", true, false, false, null);

    channel.QueueBind("my.queue1", "ex.topic", "*.image.*");
    channel.QueueBind("my.queue2", "ex.topic", "#.image");
    channel.QueueBind("my.queue3", "ex.topic", "image.#");

    channel.BasicPublish("ex.topic", "convert.image.bmp", null,
        Encoding.UTF8.GetBytes("Routing key is: convert.image.bmp"));

    channel.BasicPublish("ex.topic", "convert.bitmap.image", null,
        Encoding.UTF8.GetBytes("Routing key is: convert.bitmap.image"));

    channel.BasicPublish("ex.topic", "image.bitmap.32bit", null,
        Encoding.UTF8.GetBytes("Routing key is: image.bitmap.32bit"));

    Console.WriteLine("Awaiting...");
    Console.ReadLine();

    //cleaning up for other demos
    channel.QueueDelete("my.queue1");
    channel.QueueDelete("my.queue2");
    channel.QueueDelete("my.queue3");

    channel.ExchangeDelete("ex.topic");

    channel.Close();
    connection.Close();
}
