using System.Linq.Expressions;
using System.Reflection;
using Ploch.Common.Linq;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi;

public static class ManagementObjectQuery
{
    public static async Task<IEnumerable<TManagementObject>> GetAllAsync<TManagementObject, TPropertyValue>(this IWmiQuery wmiQuery,
                                                                                                            Expression<Func<TManagementObject,
                                                                                                                    TPropertyValue>>
                                                                                                                wherePropertySelectorExpression,
                                                                                                            TPropertyValue propertyValue,
                                                                                                            CancellationToken cancellationToken =
                                                                                                                default)
        where TManagementObject : new()
    {
        var whereClause = GetWhereClause(wherePropertySelectorExpression, propertyValue);

        return await GetAllAsync<TManagementObject>(wmiQuery, whereClause, cancellationToken);
    }

    private static string GetWhereClause<TManagementObject, TPropertyValue>(
        Expression<Func<TManagementObject, TPropertyValue>> wherePropertySelectorExpression,
        TPropertyValue propertyValue)
    {
        var propertyName = wherePropertySelectorExpression.GetMemberName();

        var whereClause = $"where {propertyName} = '{propertyValue}'";

        return whereClause;
    }

    public static Task<IEnumerable<TManagementObject>> GetAllAsync<TManagementObject>(this IWmiQuery wmiQuery,
                                                                                      string? whereClause = null,
                                                                                      CancellationToken cancellationToken = default)
        where TManagementObject : new() =>
        Task.Run(() => wmiQuery.GetAll<TManagementObject>(whereClause), cancellationToken);

    public static IEnumerable<TManagementObject> GetAll<TManagementObject>(this IWmiQuery wmiQuery, string? whereClause = null)
        where TManagementObject : new()
    {
        var query = GetQueryResults<TManagementObject>(wmiQuery, whereClause);

        var results = new List<TManagementObject>();
        foreach (var wmiObject in query)
        {
            var resultObject = ManagementObjectBuilder.BuildObject<TManagementObject>(wmiObject);

            results.Add(resultObject);
        }

        return results;
    }

    public static Task<TManagementObject?> GetFirstOrDefaultAsync<TManagementObject, TPropertyValue>(this IWmiQuery wmiQuery,
                                                                                                     Expression<Func<TManagementObject,
                                                                                                             TPropertyValue>>
                                                                                                         wherePropertySelectorExpression,
                                                                                                     TPropertyValue propertyValue,
                                                                                                     CancellationToken cancellationToken = default)
        where TManagementObject : new() =>
        Task.Run(() => wmiQuery.GetFirstOrDefault(wherePropertySelectorExpression, propertyValue), cancellationToken);

    public static TManagementObject? GetFirstOrDefault<TManagementObject, TPropertyValue>(this IWmiQuery wmiQuery,
                                                                                          Expression<Func<TManagementObject, TPropertyValue>>
                                                                                              wherePropertySelectorExpression,
                                                                                          TPropertyValue propertyValue)
        where TManagementObject : new()
    {
        var whereClause = GetWhereClause(wherePropertySelectorExpression, propertyValue);

        var query = GetQueryResults<TManagementObject>(wmiQuery, whereClause);
        foreach (var wmiObject in query)
        {
            return ManagementObjectBuilder.BuildObject<TManagementObject>(wmiObject);
        }

        return default;
    }

    public static TManagementObject? GetFirstOrDefault<TManagementObject>(this IWmiQuery wmiQuery, string? whereClause = null)
        where TManagementObject : new()
    {
        var query = GetQueryResults<TManagementObject>(wmiQuery, whereClause);
        foreach (var wmiObject in query)
        {
            return ManagementObjectBuilder.BuildObject<TManagementObject>(wmiObject);
        }

        return default;
    }

    private static IEnumerable<IWmiObject> GetQueryResults<TManagementObject>(this IWmiQuery wmiQuery, string? whereClause = null)
        where TManagementObject : new()
    {
        var objectType = typeof(TManagementObject);
        var managementClassAttribute = ValidateManagementObjectType(objectType);

        var queryText = CreateQuery(managementClassAttribute, whereClause);

        return wmiQuery.Execute(queryText);
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
