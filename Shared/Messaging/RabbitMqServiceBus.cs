using System.Diagnostics;
using System.Text.Json;
using EasyNetQ;
using Monitoring;
using Shared.Messaging.Topics;

namespace Shared.Messaging;

public class RabbitMqServiceBus(IBus bus) : IMessageBus
{
    
    private readonly IBus _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    private readonly IAdvancedBus _advancedBus = bus.Advanced;
    public async Task PublishAsync<T>(MessageTopic topic, T message) where T : TracedMessage
    {

        using var activity =
            LoggingService.activitySource.StartActivity("Publishing event: " + Enum.GetName(topic),
                ActivityKind.Producer);
        message.TraceContext = new Dictionary<string, string>
        {
            ["traceparent"] = activity?.Id ?? "",
            ["traceid"] = activity?.TraceId.ToString() ?? "",
            ["spanid"] = activity?.SpanId.ToString() ?? "",
            ["correlationid"] = activity?.TraceId.ToString() ?? ""
        };
    
        //TODO: Add retry or circuit breaker logic 
        LoggingService.Log.Information("Publishing message to RabbitMQ, Topic: " + Enum.GetName(topic));

        await _bus.PubSub.PublishAsync(message,Enum.GetName(topic));
    }

   
    public async Task SubscribeAsync<T>(MessageTopic topic, string subscriberId, Action<T> handler) where T : TracedMessage
    {
        var subscriptionId = $"{Enum.GetName(topic)}-{subscriberId}";
        Console.WriteLine("Subscription to RabbitMQ, Topic: " + subscriptionId);
        await _bus.PubSub.SubscribeAsync<T>(subscriptionId, async message =>
        {
            var traceParent = ActivityHelper.ExtractPropagationContextFromMessage(message.TraceContext);
            using var activity = LoggingService.activitySource.StartActivity("Handle Message event: " + Enum.GetName(topic), ActivityKind.Consumer, traceParent.ActivityContext);
            
            LoggingService.Log.Information("Received message of type: " + (message?.GetType()));

            try
            {
                handler(message);
            }
            catch(Exception e)
            {
                LoggingService.Log.Error(e,"Failed to hande message");
            }
        });
    }
}