using System.Linq;

namespace Ploch.Common.Data.GenericRepository;

public interface IQueryableRepository<TEntity>
    where TEntity : class
{
    IQueryable<TEntity> Entities { get; }

    IQueryable<TEntity> GetPageQuery(int pageNumber, int pageSize);
}