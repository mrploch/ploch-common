using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;


public class ReadRepositoryAsync<TEntity> : QueryableRepository<TEntity>, IReadRepositoryAsync<TEntity>
    where TEntity : class
{
    public async Task<TEntity> GetByIdAsync(object[] keyValues, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(keyValues, cancellationToken);
    }

    public async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Entities.ToListAsync(cancellationToken);
    }

    public async Task<IList<TEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPageQuery(pageNumber, pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return Entities.CountAsync(cancellationToken);
    }

    public ReadRepositoryAsync(DbContext dbContext) : base(dbContext)
    {
    }
}
public class ReadRepositoryAsync<TEntity, TId> : ReadRepositoryAsync<TEntity>, IReadRepositoryAsync<TEntity, TId>
    where TEntity : class, IHasId<TId>
{
    public ReadRepositoryAsync(DbContext dbContext) : base(dbContext)
    { }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object?[] { id }, cancellationToken);
    }
}