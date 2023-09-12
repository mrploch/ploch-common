namespace Ploch.Common.Data.Model;

public interface IEntityWithExtendedAttributes<TExtendedAttribute>
{
    public ICollection<TExtendedAttribute> ExtendedAttributes { get; set; }
}