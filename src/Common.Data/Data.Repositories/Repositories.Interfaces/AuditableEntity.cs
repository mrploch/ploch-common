namespace Ploch.Common.Data.Repositories.Interfaces
{
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; } = default!;

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; } = null!;

        public DateTime? LastModifiedOn { get; set; }
    }
}