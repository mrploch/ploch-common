using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;

public interface IReadWriteRepository<TEntity, in TId> : IReadRepository<TEntity, TId>, IWriteRepository<TEntity, TId>
    where TEntity : class, IHasId<TId>
{ }