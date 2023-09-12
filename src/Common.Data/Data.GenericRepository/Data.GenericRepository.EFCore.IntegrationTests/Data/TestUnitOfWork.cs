namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;

public class TestUnitOfWork : UnitOfWork<TestDbContext>
{
    public TestUnitOfWork(IServiceProvider serviceProvider, TestDbContext dbContext) : base(serviceProvider, dbContext)
    { }
}