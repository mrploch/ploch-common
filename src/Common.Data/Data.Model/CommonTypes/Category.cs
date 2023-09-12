using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model.CommonTypes;

public class Category<TCategory> : Category<TCategory, int> where TCategory : Category<TCategory, int>
{ }

public class Category<TCategory, TId> : IHasId<TId>, INamed, IHierarchicalWithParentComposite<TCategory>, IHierarchicalWithChildrenComposite<TCategory>
    where TCategory : Category<TCategory, TId>
{
    [Key]
    public TId Id { get; set; } = default!;

    public virtual ICollection<TCategory>? Children { get; set; }

    public virtual TCategory? Parent { get; set; }

    [MaxLength(128)]
    public string? Name { get; set; }
}