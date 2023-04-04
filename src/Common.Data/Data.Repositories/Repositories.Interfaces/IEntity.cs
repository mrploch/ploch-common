namespace Ploch.Common.Data.Repositories.Interfaces
{
    public interface IEntity<TId> : IEntity
    {
        public TId Id { get; set; }
    }

    public interface IEntity
    { }
}