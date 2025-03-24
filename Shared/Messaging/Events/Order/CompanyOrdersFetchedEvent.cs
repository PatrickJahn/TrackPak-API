namespace Shared.Messaging.Events.Order;

public class CompanyOrdersFetchedEvent
{
    public Guid CompanyId { get; set; }
    public List<Guid> OrderIds { get; set; } = [];
}