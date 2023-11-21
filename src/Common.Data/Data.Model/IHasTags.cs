using Ploch.Common.Data.Model.CommonTypes;

namespace Ploch.Common.Data.Model;

public interface IHasTags<TTag, TTagId> where TTag : Tag<TTagId>
{
    ICollection<TTag> Tags { get; set; }
}

public interface IHasTags<TTag> : IHasTags<TTag, int> where TTag : Tag<int>
{ }