namespace Ploch.Common.Data.Model
{
    public interface IHierarchicalWithParent<T>
    {
        T Parent { get; set; }
    }
}