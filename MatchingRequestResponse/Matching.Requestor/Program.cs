using Matching.Common.Consts;
using Matching.Common.Enums;
using Matching.Common.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

var waitingRequests = new ConcurrentDictionary<string, CalculationRequest>();

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
    //using default exchange!
    var responseQueueName = Guid.NewGuid().ToString();
    channel.QueueDeclare(responseQueueName);
    //channel.QueueDeclare("requests");

    channel.QueueDeclare("requests", true, false, true, null);
    //channel.QueueDeclare("responses", true, false, false, null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, e) =>
    {
        var requestId = e.BasicProperties.CorrelationId;

        //alternative version: instead using correlationid, you may use headers
        //var requestId = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers[MatchingConstants.RequestIdHeaderKey]);

        if (waitingRequests.TryGetValue(requestId, out var request))
        {
            var messageData = Encoding.UTF8.GetString(e.Body.ToArray());
            var response = JsonConvert.DeserializeObject<CalculationResponse>(messageData);

            Console.WriteLine($"Calculation Result: {request} = {response}");
        }

        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine($"\nMessage received: {message}\n");
    };

    channel.BasicConsume(responseQueueName, true, consumer);

    Console.WriteLine("Press a key to send requests");
    Console.ReadKey();

    SendRequest(waitingRequests, channel, responseQueueName, new CalculationRequest(2, 4, OperationType.Add));
    SendRequest(waitingRequests, channel, responseQueueName, new CalculationRequest(4, 4, OperationType.Multiply));
    SendRequest(waitingRequests, channel, responseQueueName, new CalculationRequest(30, 17, OperationType.Subtract));
    SendRequest(waitingRequests, channel, responseQueueName, new CalculationRequest(15, 3, OperationType.Divide));

    Console.ReadKey();

    //channel.QueueDelete("requests");
    //channel.QueueDelete("responses");

    channel.Close();
    connection.Close();
}

void SendRequest(ConcurrentDictionary<string, CalculationRequest> waitingRequests,
    IModel channel, string replyQueueName, CalculationRequest request)
{
    var requestId = Guid.NewGuid().ToString();
    var requestData = JsonConvert.SerializeObject(request);

    waitingRequests[requestId] = request;

    var basicProperties = channel.CreateBasicProperties();
    basicProperties.CorrelationId = requestId;
    basicProperties.ReplyTo = replyQueueName;

    //alternative version: instead using correlationid, you may use headers
    /*basicProperties.Headers = new Dictionary<string, object>
    {
        { MatchingConstants.RequestIdHeaderKey, Encoding.UTF8.GetBytes(requestId)},
        { MatchingConstants.ResponseQueueHeaderKey, Encoding.UTF8.GetBytes(responseQueueName)},
    };*/

    channel.BasicPublish("", "requests", basicProperties, Encoding.UTF8.GetBytes(requestData));
}