using Matching.Common.Consts;
using Matching.Common.Enums;
using Matching.Common.Models;
using Newtonsoft.Json;
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
    consumer.Received += (sender, e) =>
    {
        var requestData = Encoding.UTF8.GetString(e.Body.ToArray());
        var request = JsonConvert.DeserializeObject<CalculationRequest>(requestData);

        Console.WriteLine($"Request received: {request}");

        var response = new CalculationResponse();

        switch (request.Operation)
        {
            case OperationType.Add: 
                response.Result = request.Number1 + request.Number2;
                break;
            case OperationType.Subtract:
                response.Result = request.Number1 - request.Number2;
                break;
            case OperationType.Multiply:
                response.Result = request.Number1 * request.Number2;
                break;
            case OperationType.Divide:
                response.Result = request.Number1 / request.Number2;
                break;
            default:
                break;
        }

        var responseData = JsonConvert.SerializeObject(response);
        var basicProperties = channel.CreateBasicProperties();
        basicProperties.CorrelationId = e.BasicProperties.CorrelationId;

        //alternative version: instead using correlationid, you may use headers
        /*basicProperties.Headers = new Dictionary<string, object>
        {
            { MatchingConstants.RequestIdHeaderKey, e.BasicProperties.Headers[MatchingConstants.RequestIdHeaderKey]}
        };*/

        var replyQueue = e.BasicProperties.ReplyTo;
        //var replyQueue = e.BasicProperties.Headers[MatchingConstants.ResponseQueueHeaderKey];

        //channel.BasicPublish("", "responses", basicProperties, Encoding.UTF8.GetBytes(responseData));
        channel.BasicPublish("", replyQueue, basicProperties, Encoding.UTF8.GetBytes(responseData));
    };

    channel.BasicConsume("requests", true, consumer);

    Console.WriteLine("Press any key to exit");
    Console.ReadKey();

    channel.Close();
    connection.Close();
}
