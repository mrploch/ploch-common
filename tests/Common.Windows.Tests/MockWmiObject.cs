using Ploch.Common.Windows.Wmi;
using WmiLight;

namespace Ploch.Common.Windows.Tests;

public class MockWmiObject : IWmiObject
{
    private readonly IDictionary<string, object?> _properties;

    public MockWmiObject(params IEnumerable<(string, object?)> properties) => _properties = properties.ToDictionary(x => x.Item1, x => x.Item2);

    public WmiObjectGenus Genus { get; set; }

    public string Class { get; set; } = null!;

    public string SuperClass { get; set; } = null!;

    public string Dynasty { get; set; } = null!;

    public string Namespace { get; set; } = null!;

    public object? this[string propertyName] => _properties[propertyName];

    public object? GetPropertyValue(string propertyName) => _properties[propertyName];

    public TResult? GetPropertyValue<TResult>(string propertyName) => throw new NotImplementedException();

    public object ExecuteMethod(WmiMethod method, out WmiMethodParameters outParameters) => throw new NotImplementedException();

    public TReturnValue ExecuteMethod<TReturnValue>(WmiMethod method, out WmiMethodParameters outParameters) => throw new NotImplementedException();

    public object ExecuteMethod(WmiMethod method, WmiMethodParameters inParameters, out WmiMethodParameters outParameters) =>
        throw new NotImplementedException();

    public TReturnValue ExecuteMethod<TReturnValue>(WmiMethod method, WmiMethodParameters inParameters, out WmiMethodParameters outParameters) =>
        throw new NotImplementedException();

    public IWmiMethod GetMethod(string methodName) => throw new NotImplementedException();

    public IEnumerable<string> GetPropertyNames() => _properties.Keys;

    public IEnumerable<(string, object?)> GetProperties() => _properties.Select(pair => (pair.Key, pair.Value));

    public WmiObject GetWmiObject() => throw new NotImplementedException();
}