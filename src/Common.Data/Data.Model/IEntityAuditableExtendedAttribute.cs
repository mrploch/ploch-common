namespace Ploch.Common.Data.Model;

public interface IEntityAuditableExtendedAttribute<TId, TEntityId, TEntity> : IEntityExtendedAttribute<TId, TEntityId>, IAuditableEntity<TId>
    where TEntity : IHasId<TEntityId>
{ }

public interface IEntityAuditableExtendedAttribute<TEntityId, TEntity> : IEntityExtendedAttribute<TEntityId, TEntity>, IAuditableEntity where TEntity : IHasId<TEntityId>
{ }

public interface IEntityAuditableExtendedAttribute : IEntityExtendedAttribute, IAuditableEntity
{ }