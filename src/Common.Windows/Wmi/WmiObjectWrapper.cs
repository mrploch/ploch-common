using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public class WmiObjectWrapper(WmiObject wmiObject) : IWmiObject
{
    public object? this[string propertyName] => wmiObject.GetPropertyValue(propertyName);

    public object? GetPropertyValue(string propertyName) => wmiObject.GetPropertyValue(propertyName);

    public IEnumerable<string> GetPropertyNames() => wmiObject.GetPropertyNames();

    public IEnumerable<(string, object?)> GetProperties() => GetPropertyNames().Select(propertyName => (propertyName, GetPropertyValue(propertyName)));

    public TValue? GetPropertyValue<TValue>(string propertyName) => (TValue)wmiObject.GetPropertyValue(propertyName);
}
