using System.Collections.Generic;

namespace Ploch.Common.Data.Model
{
    public interface IHierarchicalWithChildren<T>
    {
        ICollection<T> Children { get; set; }
    }
}