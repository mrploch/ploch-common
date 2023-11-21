using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model.CommonTypes;

public class Tag<TId> : IHasId<TId>, INamed, IHasDescription
{
    [Key]
    public TId Id { get; set; } = default!;

    [MaxLength(128)]
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}

public class Tag : Tag<int>
{ }