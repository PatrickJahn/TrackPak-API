using Shared.Models;

namespace EmployeeService.Application.Models;

public record CreateEmployeeModel()
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }
  public string PhoneNumber { get; set; }
  public CreateLocationRequestModel Location { get; set; }
}