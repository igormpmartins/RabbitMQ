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
    channel.ExchangeDeclare("exchange1", "direct", true, false, null);
    channel.ExchangeDeclare("exchange2", "direct", true, false, null);

    channel.QueueDeclare("my.queue1", true, false, false, null);
    channel.QueueDeclare("my.queue2", true, false, false, null);

    channel.QueueBind("my.queue1", "exchange1", "key1");
    channel.QueueBind("my.queue2", "exchange2", "key2");

    channel.ExchangeBind("exchange2", "exchange1", "key2");

    channel.BasicPublish("exchange1", "key1", null, Encoding.UTF8.GetBytes("Msg for Key 1"));

    //This will also send msg to queue 2, because exchange 1 is binded to exchange 2.
    channel.BasicPublish("exchange1", "key2", null, Encoding.UTF8.GetBytes("Msg for Key 2"));

    Console.WriteLine("Awaiting...");
    Console.ReadLine();

    //cleaning up for other demos
    channel.QueueDelete("my.queue1");
    channel.QueueDelete("my.queue2");

    channel.ExchangeDelete("exchange1");
    channel.ExchangeDelete("exchange2");

    channel.Close();
    connection.Close();
}
