using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model.CommonTypes
{
    public class Category<TCategory> : IHasId<int>, INamed, IHierarchicalWithParentComposite<TCategory>, IHierarchicalWithChildrenComposite<TCategory>
        where TCategory : Category<TCategory>
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<TCategory>? Children { get; set; }

        public virtual TCategory? Parent { get; set; }

        [MaxLength(128)]
        public string? Name { get; set; }
    }
}