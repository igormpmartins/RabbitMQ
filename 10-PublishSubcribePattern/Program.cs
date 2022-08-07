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

    channel.QueueDeclare("my.queue1", true, false, false, null);
    channel.QueueDeclare("my.queue2", true, false, false, null);

    channel.QueueBind("my.queue1", "ex.fanout", "");
    channel.QueueBind("my.queue2", "ex.fanout", "");

    while (true)
    {
        Console.WriteLine("<Type 'exit' to finish application>");
        Console.WriteLine("Enter message:");
        
        var message = Console.ReadLine();
        if (message.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            break;

        channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes(message));
    }

    channel.QueueDelete("my.queue1");
    channel.QueueDelete("my.queue2");
    channel.ExchangeDelete("ex.fanout");

    channel.Close();
    connection.Close();
}
