using CompanyService.Domain.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Events.Location;

namespace CompanyService.Application.Messaging.Handlers;

public class CompanyLocationCreatedHandler(ICompanyRepository companyRepository)
    : IMessageHandler<CompanyLocationCreatedEvent>
{
    public async Task HandleAsync(CompanyLocationCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Updating company {message.CompanyId} with location {message.LocationId}");

        var company = await companyRepository.GetOrDefaultByIdAsync(message.CompanyId);

        if (company == null)
        {
            // TODO: Add logic 
            return;
        }

        company.LocationId = message.LocationId;
        await companyRepository.Update(company);
    }
}