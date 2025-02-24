using Shared.Models;

namespace Shared.Messaging.Events.User;

public class UserCreatedEvent
{
    
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CreateLocationRequestModel Location { get; set; }
}