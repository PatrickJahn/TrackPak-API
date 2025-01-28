using System.ComponentModel.DataAnnotations.Schema;
using Shared.Entities;
using Shared.Interfaces;

namespace UserService.Domain.entities;

public class User : BaseModel, ISoftDeleteEntity, IAuditableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public Guid? LocationId { get; set; } 
    
    
    [NotMapped]
    public string DisplayName => $"{FirstName} {LastName}";


    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}