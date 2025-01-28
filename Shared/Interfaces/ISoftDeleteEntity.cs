namespace Shared.Interfaces;


public interface ISoftDeleteEntity {
    DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}