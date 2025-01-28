using System.ComponentModel.DataAnnotations.Schema;
using Shared.Interfaces;

namespace UserService.Domain.entities;

public class User : IBaseEntity, ISoftDeleteEntity, IAuditableEntity
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    
    
    public List<Guid> LocationIds { get; set; }
    
    
    
    [NotMapped]
    public string DisplayName => $"{FirstName} {LastName}";


    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}