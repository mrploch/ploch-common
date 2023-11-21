using Ploch.Common.Data.Model.CommonTypes;

namespace Ploch.Common.Data.Model;

public interface IHasCategories<TCategory, TCategoryId> where TCategory : Category<TCategory, TCategoryId>
{
    ICollection<TCategory> Categories { get; set; }
}

public interface IHasCategories<TCategory> : IHasCategories<TCategory, int> where TCategory : Category<TCategory, int>
{ }