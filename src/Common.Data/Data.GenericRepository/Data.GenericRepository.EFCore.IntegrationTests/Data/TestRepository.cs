using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;

public class TestRepository<TEntity, TId> : RepositoryAsync<TestDbContext, TEntity, TId> where TEntity : class, IHasId<TId>
{
    public TestRepository(TestDbContext dbContext) : base(dbContext)
    { }
}