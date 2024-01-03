using System.Reflection;

namespace Ploch.Common.Reflection;

public static class ObjectReflectionExtensions
{
    public static object GetFieldValue(this object obj, string fieldName)
    {
        var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        return fieldInfo.GetValue(obj);
    }

    public static TValue GetFieldValue<TValue>(this object obj, string fieldName)
    {
        return (TValue)obj.GetFieldValue(fieldName);
    }
}