using CompanyService.Application.Interfaces;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;
using Shared.Messaging;
using Shared.Messaging.Events.Company;
using Shared.Messaging.Topics;
using Shared.Models;

namespace CompanyService.Infrastructure.Messaging;

public class CompanyEventPublisher(IMessageBus messageBus) : ICompanyEventPublisher
{
    public  async Task PublishCompanyCreatedAsync(Company company, CreateLocationRequestModel locationRequest)
    {

        var @event = new CompanyCreatedEvent()
        {
            Location = locationRequest,
            CompanyId = company.Id
        };

        await messageBus.PublishAsync(MessageTopic.CompanyCreated, @event);
    }
    
}