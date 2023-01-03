using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model
{
    public class Category<TCategory> : IHasId<int>,
                                       INamed,
                                       IHierarchicalWithParentComposite<TCategory>,
                                       IHierarchicalWithChildrenComposite<TCategory> where TCategory : IHierarchicalWithChildren<TCategory>, IHierarchicalWithParent<TCategory>
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public virtual TCategory? Parent { get; set; }

        public virtual ICollection<TCategory>? Children { get; set; }
    }
}