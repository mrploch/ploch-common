using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model;

public interface IHasIdSettable<TId> : IHasId<TId>
{
    [Key]
    new TId Id { get; set; }
}