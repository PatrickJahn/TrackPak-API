using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;
using Shared.Models.Enums;

namespace EmployeeService.Domain.Entities;

public class Employee : BaseModel
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }
  public string PhoneNumber { get; set; }

  public Boolean CheckedIn { get; set; }
  
  public UserRole Role { get; set; }

  public Guid CompanyId { get; set; }
  public Guid? LocationId { get; set; }

  [NotMapped]
  public string DisplayName => $"{FirstName} {LastName}";
}

