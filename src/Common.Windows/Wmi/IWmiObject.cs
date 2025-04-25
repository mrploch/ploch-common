namespace Ploch.Common.Windows.Wmi;

public interface IWmiObject
{
    object? this[string propertyName] { get; }

    object? GetPropertyValue(string propertyName);

    IEnumerable<string> GetPropertyNames();

    public IEnumerable<(string, object?)> GetProperties();
}
