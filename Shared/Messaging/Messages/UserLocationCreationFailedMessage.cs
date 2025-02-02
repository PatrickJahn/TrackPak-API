namespace Shared.Messaging.Messages;

public class UserLocationCreationFailedMessage
{
    public string Country { get; set; }
    public string City { get; set; }
    public string AddressLine { get; set; }
    public string PostalCode { get; set; }
}