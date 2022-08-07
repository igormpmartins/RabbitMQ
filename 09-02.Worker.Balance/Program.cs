using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Work balance example");
Console.WriteLine();

Console.WriteLine("Enter worker name:");
var worker = Console.ReadLine();
Console.WriteLine();

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

    //create manually, otherwise it wont be possible to run multiple workers

    //channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);
    //channel.QueueDeclare("my.queue1", true, false, false, null);
    //channel.QueueBind("my.queue1", "ex.fanout", "");

    CreateConsumer(channel);

    //Use web interface for sending messages

    //remove manually after running the demo, otherwise first worker will affect others!
    //channel.QueueDelete("my.queue1");
    //channel.ExchangeDelete("ex.fanout");

    channel.Close();
    connection.Close();
}

void CreateConsumer(IModel channel)
{
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, args) =>
    {
        var message = Encoding.UTF8.GetString(args.Body.ToArray());
        int.TryParse(message, out var taskTime);

        Console.Write($"[Worker {worker}] Starting task ({taskTime})...");
        Thread.Sleep(taskTime * 1000);
        Console.WriteLine("finished");

        channel.BasicAck(args.DeliveryTag, false);
    };

    var consumerTag = channel.BasicConsume("my.queue1", false, consumer);

    Console.WriteLine("Awaiting, press any key to exit...");
    Console.ReadLine();

    channel.BasicCancel(consumerTag);
}