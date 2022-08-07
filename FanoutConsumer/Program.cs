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
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += Consumer_Received;

    //Auto Ack
    //var consumerTag = channel.BasicConsume("my.queue1", true, consumer);
    
    //Without auto Ack
    var consumerTag = channel.BasicConsume("my.queue1", false, consumer);

    Console.WriteLine("Waiting for messages. Press any key to exit.");
    Console.ReadLine();

    channel.Close();
    connection.Close();


    //local method
    void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine($"Received message: {message}");
        
        //manual Ack
        //channel.BasicAck(e.DeliveryTag, false);

        //NAck, not putting back to the queue!
        channel.BasicNack(e.DeliveryTag, false, false);
    }
}

