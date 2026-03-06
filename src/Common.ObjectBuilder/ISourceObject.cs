namespace Ploch.Common.ObjectBuilder;

public interface ISourceObject
{
    IEnumerable<string> GetPropertyNames();

    object GetPropertyValue(string propertyName);

    IDictionary<string, object> GetProperties();
}
