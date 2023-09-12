namespace Ploch.Common.Data.Model;

public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
{
    public TId Id { get; set; } = default!;

    public DateTime? CreatedTime { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedTime { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? AccessedTime { get; set; }

    public string? LastAccessedBy { get; set; }
}