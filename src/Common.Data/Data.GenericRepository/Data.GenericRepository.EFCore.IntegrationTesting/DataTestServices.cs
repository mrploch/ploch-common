using System.Data.Common;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;

public class DataTestServices
{
    private readonly DbConnection _connection;

    public DataTestServices(DbConnection connection)
    {
        _connection = connection;
    }
}