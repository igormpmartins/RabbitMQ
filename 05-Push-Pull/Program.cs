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
    channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);

    channel.QueueDeclare("my.queue1", true, false, false, null);
    channel.QueueBind("my.queue1", "ex.fanout", "");

    //choose model!
    //ReadMessagesWithPushModel();
    ReadMessagesWithPullModel();

    //Use web interface for sending messages

    //cleaning up for other demos
    channel.QueueDelete("my.queue1");
    channel.ExchangeDelete("ex.fanout");

    channel.Close();
    connection.Close();

    void ReadMessagesWithPushModel()
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, args) =>
        {
            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            Console.WriteLine($"Push Message - {message}");
        };

        var consumerTag = channel.BasicConsume("my.queue1", true, consumer);

        Console.WriteLine("Awaiting, press any key to exit...");
        Console.ReadLine();

        channel.BasicCancel(consumerTag);
    }

    void ReadMessagesWithPullModel()
    {
        Console.WriteLine("Awaiting, press 'e' to exit...");

        while (true)
        {
            Console.WriteLine("Reading messages for queue...");

            var result = channel.BasicGet("my.queue1", true);
            if (result != null)
            {
                var message = Encoding.UTF8.GetString(result.Body.ToArray());
                Console.WriteLine($"Pull Message - {message}");
            }

            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey();
                if (char.ToLower(keyInfo.KeyChar) == 'e')
                    return;
            }

            Thread.Sleep(2000);
        }
    }

}

