namespace Shared.Messaging.Events.Location;

public class CompanyLocationCreatedEvent
{
    public Guid CompanyId { get; set; }
    public Guid LocationId { get; set; }
}