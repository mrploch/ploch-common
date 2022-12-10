namespace Ploch.Common.Data.Model
{
    /// <summary>
    ///     Defines a type that has an identifier.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface IHasId<out TId>
    {
        TId Id { get; }
    }

    public interface IHasIdSettable<TId> : IHasId<TId>
    {
        new TId Id { get; set; }
    }
}