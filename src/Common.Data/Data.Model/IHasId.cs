using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model;

/// <summary>
///     Defines an entity that has an <c>Id</c> property.
/// </summary>
/// <remarks>
///     Defines an entity with an <c>Id</c> property which is usually used as a primary key.
/// </remarks>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IHasId<out TId> : IEntity
{
    /// <summary>
    ///     The entity identifier.
    /// </summary>
    [Key]
    TId Id { get; }
}