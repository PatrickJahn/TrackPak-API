using Shared.Interfaces;

namespace Shared.Entities;

public abstract class BaseModel : IBaseEntity {
    public  Guid Id { get; set; } = Guid.NewGuid();
}
