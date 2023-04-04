using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model
{
    public class Tag : IHasId<int>, INamed, IHasDescription
    {
        public string? Description { get; set; }

        [Key]
        public int Id { get; }

        [MaxLength(128)]
        [Required]
        public string Name { get; set; } = null!;
    }
}