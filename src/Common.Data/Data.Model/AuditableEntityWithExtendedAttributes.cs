﻿namespace Ploch.Common.Data.Model;

public abstract class AuditableEntityWithExtendedAttributes<TId, TEntityId, TEntity, TExtendedAttribute> : AuditableEntity<TEntityId>,
                                                                                                           IEntityWithExtendedAttributes<TExtendedAttribute>
    where TEntity : IHasId<TEntityId>
{
    public AuditableEntityWithExtendedAttributes()
    {
        ExtendedAttributes = new HashSet<TExtendedAttribute>();
    }

    public virtual ICollection<TExtendedAttribute> ExtendedAttributes { get; set; }
}