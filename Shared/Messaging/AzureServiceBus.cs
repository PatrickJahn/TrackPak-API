using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;
using Shared.Messaging.Topics;

namespace Shared.Messaging;


public class AzureServiceBus(string connectionString) : IMessageBus
{
    private readonly ServiceBusClient _client = new ServiceBusClient(connectionString);

    public async Task PublishAsync<T>(MessageTopic topic, T message)
    {
        var sender = _client.CreateSender(Enum.GetName(topic));

        var serviceBusMessage = new ServiceBusMessage(message?.ToString());
        await sender.SendMessageAsync(serviceBusMessage);    }

    public async Task SubscribeAsync<T>(MessageTopic topic, Action<T> handler)
    {
        // TODO: Check up on subscriptionName 
        var processor = _client.CreateProcessor(Enum.GetName(topic), "all");
        processor.ProcessMessageAsync += async (args) =>
        {
            // Deserialize message and invoke handler - Maybe use json converter instead? 
            var message = args.Message.Body.ToString();
             handler((T)Convert.ChangeType(message, typeof(T)));
        };
        
        processor.ProcessErrorAsync += async args =>
        {
            Console.WriteLine($"Error: {args.Exception.Message}");
        };

        await processor.StartProcessingAsync();
    }
    
}
