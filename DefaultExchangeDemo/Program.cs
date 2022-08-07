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
    channel.QueueDeclare("my.queue1", true, false, false, null);
    channel.QueueDeclare("my.queue2", true, false, false, null);

    channel.BasicPublish("", "my.queue1", null, Encoding.UTF8.GetBytes("msg q1"));
    channel.BasicPublish("", "my.queue2", null, Encoding.UTF8.GetBytes("msg q2"));

    Console.WriteLine("Awaiting...");
    Console.ReadLine();

    //cleaning up for other demos
    channel.QueueDelete("my.queue1");
    channel.QueueDelete("my.queue2");

    channel.Close();
    connection.Close();
}
