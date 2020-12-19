namespace Ploch.Common.Data.Model
{
    public interface IHierarchicalWithChildrenComposite<T> : IHierarchicalWithChildren<T> where T : IHierarchicalWithChildren<T>
    { }
}