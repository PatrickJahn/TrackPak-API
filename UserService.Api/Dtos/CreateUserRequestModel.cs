using Shared.Entities;

namespace UserService.Api.Dtos;

public class CreateUserRequestModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Address Address { get; set; }
    
}