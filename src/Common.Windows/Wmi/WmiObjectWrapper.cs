using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public class WmiObjectWrapper(WmiObject wmiObject) : IWmiObject
{
    public WmiObjectGenus Genus => wmiObject.Genus;

    public string Class => wmiObject.Class;

    public string SuperClass => wmiObject.Class;

    public string Dynasty => wmiObject.Class;

    public string Namespace => wmiObject.Class;

    public object? this[string propertyName] => wmiObject.GetPropertyValue(propertyName);

    public object? GetPropertyValue(string propertyName) => wmiObject.GetPropertyValue(propertyName);

    public IEnumerable<string> GetPropertyNames() => wmiObject.GetPropertyNames();

    public IEnumerable<(string, object?)> GetProperties() => GetPropertyNames().Select(propertyName => (propertyName, GetPropertyValue(propertyName)));
    public WmiObject GetWmiObject() => wmiObject;

    public TValue? GetPropertyValue<TValue>(string propertyName) => (TValue)wmiObject.GetPropertyValue(propertyName);
    public object ExecuteMethod(WmiMethod method, out WmiMethodParameters outParameters) => wmiObject.ExecuteMethod(method, out outParameters);
    public TReturnValue ExecuteMethod<TReturnValue>(WmiMethod method, out WmiMethodParameters outParameters) => throw new NotImplementedException();

    public object ExecuteMethod(WmiMethod method, WmiMethodParameters inParameters, out WmiMethodParameters outParameters) =>
        throw new NotImplementedException();

    public TReturnValue ExecuteMethod<TReturnValue>(WmiMethod method, WmiMethodParameters inParameters, out WmiMethodParameters outParameters) =>
        throw new NotImplementedException();

    public IWmiMethod GetMethod(string methodName) => new WmiMethodWrapper(wmiObject.GetMethod(methodName));
}
