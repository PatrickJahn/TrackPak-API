using System.Diagnostics;
using EasyNetQ;
using EasyNetQ.Topology;
using Monitoring;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Shared.Messaging.Topics;
using Shared.Monitoring;

namespace Shared.Messaging;

public class RabbitMqServiceBus(IBus bus, IAdvancedBus advancedBus) : IMessageBus
{
    private readonly IBus _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    private readonly IAdvancedBus _advancedBus = advancedBus ?? throw new ArgumentNullException(nameof(advancedBus));

    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (ex, time, retry, _) => LoggingService.Log.Warning($"Retry {retry} due to: {ex.Message}"));

    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy = Policy
        .Handle<Exception>()
        .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
            onBreak: (ex, ts) => LoggingService.Log.Warning($"Circuit opened: {ex.Message}"),
            onReset: () => LoggingService.Log.Information("Circuit closed."),
            onHalfOpen: () => LoggingService.Log.Information("Circuit half-open."));

    public async Task PublishAsync<T>(MessageTopic topic, T message) where T : TracedMessage
    {
        using var activity = LoggingService.activitySource.StartActivity($"Publishing event: {Enum.GetName(topic)}", ActivityKind.Producer);
        message.TraceContext = new()
        {
            ["traceparent"] = activity?.Id ?? "",
            ["traceid"] = activity?.TraceId.ToString() ?? "",
            ["spanid"] = activity?.SpanId.ToString() ?? "",
            ["correlationid"] = activity?.TraceId.ToString() ?? ""
        };

        LoggingService.Log.Information($"Publishing message to RabbitMQ, Topic: {Enum.GetName(topic)}");

        await _retryPolicy.WrapAsync(_circuitBreakerPolicy)
            .ExecuteAsync(async () =>
            {
                await _bus.PubSub.PublishAsync(message, Enum.GetName(topic) ?? string.Empty);
            });
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
                throw;
            }
        });
    }
    
}
