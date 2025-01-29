using Shared.Interfaces;

namespace Shared.Models;

public abstract class BaseModel : IBaseEntity, ISoftDeleteEntity, IAuditableEntity {
    public  Guid Id { get; set; } = Guid.NewGuid();
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
