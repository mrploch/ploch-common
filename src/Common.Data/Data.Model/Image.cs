using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model
{
    public class Image : IHasId<int>, INamed, IHasDescription
    {
        public byte[]? Contents { get; set; }

        public string? Description { get; set; }

        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}