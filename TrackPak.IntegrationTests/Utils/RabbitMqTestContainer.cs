using Shared.Messaging;
using Shared.Messaging.Topics;

namespace TrackPak.IntegrationTests.Utils;

using System.Threading.Tasks;
using Testcontainers.RabbitMq;
using Xunit;

public class RabbitMqTestContainer : IAsyncLifetime
{
    public readonly RabbitMqContainer RabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management") // Management UI enabled
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    public string ConnectionString => $"amqp://guest:guest@{RabbitMqContainer.Hostname}:{RabbitMqContainer.GetMappedPublicPort(5672)}";

    public async Task InitializeAsync() => await RabbitMqContainer.StartAsync();
    public async Task DisposeAsync() => await RabbitMqContainer.DisposeAsync();
 
}