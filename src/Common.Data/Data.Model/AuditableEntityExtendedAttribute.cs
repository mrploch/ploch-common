﻿namespace Ploch.Common.Data.Model;

public abstract class AuditableEntityExtendedAttribute<TId, TEntityId, TEntity> : AuditableEntity<TId>, IEntityAuditableExtendedAttribute<TId, TEntityId>
    where TEntity : IHasId<TEntityId> where TEntityId : IHasId<TId>
{
    /// <summary>
    ///     Extended attribute's related entity
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public virtual TEntity Entity { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <inheritdoc />
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TEntityId EntityId { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public EntityExtendedAttributeType Type { get; set; }

    /// <inheritdoc />
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Key { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public string? Text { get; set; }

    /// <inheritdoc />
    public decimal? Number { get; set; }

    /// <inheritdoc />
    public DateTime? DateTime { get; set; }

    /// <inheritdoc />
    public string? Json { get; set; }

    /// <inheritdoc />
    public string? ExternalId { get; set; }

    /// <inheritdoc />
    public string? Group { get; set; }

    /// <inheritdoc />
    public string? Description { get; set; }

    /// <inheritdoc />
    public bool IsActive { get; set; } = true;
}