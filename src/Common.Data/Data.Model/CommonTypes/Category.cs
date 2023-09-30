using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model.CommonTypes;

public class Category<TCategory> : Category<TCategory, int> where TCategory : Category<TCategory, int>
{ }

/// <summary>
///     Typical category model class.
/// </summary>
/// <remarks>
///     <para>
///         Typical category model class that can be used as a base type in data models. It has a nested structure with a
///         parent and children.
///     </para>
///     <para>
///         This type can be extended with additional properties in the application data model by inheriting from it.
///     </para>
/// </remarks>
/// <typeparam name="TCategory">The actual category type in the data model.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
public class Category<TCategory, TId> : IHasIdSettable<TId>, INamed, IHierarchicalWithParentComposite<TCategory>, IHierarchicalWithChildrenComposite<TCategory>
    where TCategory : Category<TCategory, TId>
{
    [Key]
    public TId Id { get; set; } = default!;

    public virtual ICollection<TCategory>? Children { get; set; }

    public virtual TCategory? Parent { get; set; }

    [MaxLength(128)]
    public string? Name { get; set; }
}