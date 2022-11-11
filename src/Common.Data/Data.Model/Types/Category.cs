using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model.Types
{
    public class Category : IHasId<int>, INamed, IHierarchicalWithChildrenComposite<Category>, IHierarchicalWithParentComposite<Category>
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Category>? Children { get; set; }

        public virtual Category? Parent { get; set; }
    }
}