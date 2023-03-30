using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.Data.Model
{
    /// <summary>
    ///     Defines a type that has an identifier.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface IHasId<out TId>
    {
        [Key]
        TId Id { get; }
    }

    public interface IHasIdSettable<TId> : IHasId<TId>
    {
        [Key]
        new TId Id { get; set; }
    }
}