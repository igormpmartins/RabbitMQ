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

    channel.BasicQos(0, 1, false);
        
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.Write($"\nProcessing message:\n{message}...");
        Thread.Sleep(1000);

        Console.WriteLine("finished");

        channel.BasicAck(e.DeliveryTag, false);
    };

    var consumerTag = channel.BasicConsume("my.queue", false, consumer);

    Console.WriteLine("Subscribed to the queue. Waiting for messages...");
    Console.ReadKey();

    channel.Close();
    connection.Close();
}

