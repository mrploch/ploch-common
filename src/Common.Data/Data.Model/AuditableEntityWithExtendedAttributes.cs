namespace BlazorHero.CleanArchitecture.Domain.Contracts;

public abstract class AuditableEntityWithExtendedAttributes<TId, TEntityId, TEntity, TExtendedAttribute> : AuditableEntity<TEntityId>,
                                                                                                           IEntityWithExtendedAttributes<TExtendedAttribute>
    where TEntity : IEntity<TEntityId>
{
    public AuditableEntityWithExtendedAttributes()
    {
        ExtendedAttributes = new HashSet<TExtendedAttribute>();
    }

    public virtual ICollection<TExtendedAttribute> ExtendedAttributes { get; set; }
}