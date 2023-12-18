using System.Linq;
using Dawn;
using Microsoft.EntityFrameworkCore;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class QueryableRepository<TEntity> : IQueryableRepository<TEntity>
    where TEntity : class
{
    public QueryableRepository(DbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    protected DbContext DbContext { get; }

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    public IQueryable<TEntity> Entities => DbSet;
    
    
    public IQueryable<TEntity> GetPageQuery(int pageNumber, int pageSize)
    {
        Guard.Argument(pageNumber, nameof(pageNumber)).Positive();

        return DbContext.Set<TEntity>().Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking();
    }
}