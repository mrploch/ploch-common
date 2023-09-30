using System.Linq;
using Dawn;
using Microsoft.EntityFrameworkCore;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public abstract class ReadRepositoryBase<TEntity> where TEntity : class
{
    protected ReadRepositoryBase(DbContext dbContext)
    {
        DbContext = Guard.Argument(dbContext, nameof(dbContext)).NotNull();
    }

    protected DbContext DbContext { get; }

    public IQueryable<TEntity> Entities => DbContext.Set<TEntity>();

    public IQueryable<TEntity> GetPageQuery(int pageNumber, int pageSize)
    {
        Guard.Argument(pageNumber, nameof(pageNumber)).Positive();

        return DbContext.Set<TEntity>().Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking();
    }
}