using System.Collections.Generic;

namespace Ploch.Common.Data.Model
{
    public class Category<TCategory> : IHasId<int>, 
                                       INamed,
                                       IHierarchicalWithParentComposite<TCategory>,
                                       IHierarchicalWithChildrenComposite<TCategory> where TCategory : IHierarchicalWithChildren<TCategory>, IHierarchicalWithParent<TCategory>
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public virtual TCategory? Parent { get; set; }

        public virtual ICollection<TCategory>? Children { get; set; }
    }
}