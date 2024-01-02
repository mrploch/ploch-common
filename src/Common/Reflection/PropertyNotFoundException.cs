using System;

namespace Ploch.Common.Reflection;

public class PropertyNotFoundException : Exception
{
    public PropertyNotFoundException(string propertyName) : base(GetDefaultMessage(propertyName))
    {
        PropertyName = propertyName;
    }

    public PropertyNotFoundException(string propertyName, string message, Exception innerException) : base(message, innerException)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }

    private static string GetDefaultMessage(string propertyName)
    {
        return $"Property {propertyName} was not found.";
    }
}