namespace Ploch.Common.Data.Model
{
    public interface IHierarchicalWithParentComposite<T> : IHierarchicalWithParent<T> where T : IHierarchicalWithParent<T>
    { }
}