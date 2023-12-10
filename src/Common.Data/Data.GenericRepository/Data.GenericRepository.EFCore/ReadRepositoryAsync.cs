using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class ReadRepositoryAsync<TEntity, TId> : ReadRepositoryBase<TEntity>, IReadRepositoryAsync<TEntity, TId>
    where TEntity : class, IHasId<TId>
{
    public ReadRepositoryAsync(DbContext dbContext) : base(dbContext)
    { }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Entities.ToListAsync(cancellationToken);
    }

    public async Task<IList<TEntity>> GetPagedResponseAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPageQuery(pageNumber, pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return Entities.CountAsync(cancellationToken);
    }
}