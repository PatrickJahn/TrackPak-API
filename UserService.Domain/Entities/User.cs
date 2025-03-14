using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;
using Shared.Interfaces;
using Shared.Models.Enums;

namespace UserService.Domain.entities;

public class User : BaseModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    
    public UserRole Role { get; set; }

    public Guid? LocationId { get; set; } 
    
    
    [NotMapped]
    public string DisplayName => $"{FirstName} {LastName}";


}