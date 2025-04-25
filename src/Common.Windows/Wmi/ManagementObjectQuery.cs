using System.Reflection;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;
using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public static class ManagementObjectQuery
{
    public static Task<IEnumerable<TManagementObject>> GetAllAsync<TManagementObject>(this IWmiQuery wmiQuery,
                                                                                      string? whereClause = null,
                                                                                      CancellationToken cancellationToken = default)
        where TManagementObject : new() =>
        Task.Run(() => wmiQuery.GetAll<TManagementObject>(whereClause), cancellationToken);

    public static IEnumerable<TManagementObject> GetAll<TManagementObject>(this IWmiQuery wmiQuery, string? whereClause = null)
        where TManagementObject : new()
    {
        var objectType = typeof(TManagementObject);
        var managementClassAttribute = ValidateManagementObjectType(objectType);

        var queryText = CreateQuery(managementClassAttribute, whereClause);

        var results = new List<TManagementObject>();
        using var con = new WmiConnection();
        var query = wmiQuery.Execute(queryText);

        foreach (var wmiObject in query)
        {
            var resultObject = ManagementObjectBuilder.BuildObject<TManagementObject>(wmiObject);

            results.Add(resultObject);
        }

        return results;
    }

    private static string CreateQuery(WindowsManagementClassAttribute managementClassAttribute, string? whereClause)
    {
        var queryText = $"SELECT * FROM {managementClassAttribute.ClassName}";

        if (!whereClause.IsNullOrEmpty())
        {
            queryText += $" {whereClause}";
        }

        return queryText;
    }

    private static WindowsManagementClassAttribute ValidateManagementObjectType(Type objectType)
    {
        var managementClassAttribute = objectType.GetCustomAttribute<WindowsManagementClassAttribute>(true);
        if (managementClassAttribute is null)
        {
            throw new InvalidOperationException($"Type {objectType.Name} does not have the WindowsManagementClass attribute");
        }

        return managementClassAttribute;
    }
}
