using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model.CommonTypes;

/// <summary>
///     Typical tag model class.
/// </summary>
public class Tag : IHasIdSettable<int>, INamed, IHasDescription
{
    [Key]
    public int Id { get; set; }

    [MaxLength(128)]
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}