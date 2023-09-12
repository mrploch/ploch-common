namespace Ploch.Common.Data.Model;

public interface IEntityExtendedAttribute<TId, TEntityId> : IEntityExtendedAttribute<TEntityId>, IHasId<TId>
{ }

public interface IEntityExtendedAttribute<TEntityId> : IEntityExtendedAttribute
{
    /// <summary>
    ///     External attribute's entity id
    /// </summary>
    public TEntityId EntityId { get; set; }
}

public interface IEntityExtendedAttribute
{
    /// <summary>
    ///     Extended attribute value type
    /// </summary>
    public EntityExtendedAttributeType Type { get; set; }

    /// <summary>
    ///     Extended attribute key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    ///     Value for extended attribute with type 1
    /// </summary>
    public decimal? Number { get; set; }

    /// <summary>
    ///     Value for extended attribute with type 2
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    ///     Value for extended attribute with type 3
    /// </summary>
    public DateTime? DateTime { get; set; }

    /// <summary>
    ///     Value for extended attribute with type 4
    /// </summary>
    public string? Json { get; set; }

    /// <summary>
    ///     Extended attribute external id
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    ///     Extended attribute group
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    ///     Extended attribute description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Is active
    /// </summary>
    public bool IsActive { get; set; }
}