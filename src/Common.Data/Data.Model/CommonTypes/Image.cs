using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model
{
    public class Image : IHasId<int>, INamed, IHasDescription
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string? Name { get; set; }

        public byte[]? Contents { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
    }
}