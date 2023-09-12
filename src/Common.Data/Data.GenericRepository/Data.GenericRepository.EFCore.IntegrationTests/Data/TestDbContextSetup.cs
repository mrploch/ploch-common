using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;

public static class TestDbContextSetup
{
#pragma warning disable SA1306
    private static DbConnection? Connection;
#pragma warning restore SA1306

    public static DbConnection CreateSqLiteInMemoryConnection()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        return connection;
    }

    public static TestDbContext ConfigureSqLiteInMemoryContext()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        Connection = CreateSqLiteInMemoryConnection();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var contextOptions = new DbContextOptionsBuilder<TestDbContext>().UseSqlite(Connection).Options;

        // Create the schema and seed some data
        var dbContext = new TestDbContext(contextOptions);

        return dbContext;
    }

    public static void DisposeConnection()
    {
        Connection?.Dispose();
    }
}