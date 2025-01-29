using Shared.Models;

namespace UserService.Application.Models;

public record CreateUserModel()
{
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public CreateLocationRequestModel Location { get; set; }
}