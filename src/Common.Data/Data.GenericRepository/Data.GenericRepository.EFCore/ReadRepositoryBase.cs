using System.Linq;
using Dawn;
using Microsoft.EntityFrameworkCore;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public abstract class RepositoryBase<TEntity>
    where TEntity : class
{
    protected RepositoryBase(DbContext dbContext)
    {
        DbContext = Guard.Argument(dbContext, nameof(dbContext)).NotNull();
    }

    protected DbContext DbContext { get; }

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();
}

public abstract class ReadRepositoryBase<TEntity> : RepositoryBase<TEntity>
    where TEntity : class
{
    protected ReadRepositoryBase(DbContext dbContext) : base(dbContext)
    { }

    public IQueryable<TEntity> Entities => DbSet;

    public IQueryable<TEntity> GetPageQuery(int pageNumber, int pageSize)
    {
        Guard.Argument(pageNumber, nameof(pageNumber)).Positive();

        return DbContext.Set<TEntity>().Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking();
    }
}