using System;
using System.Runtime.Serialization;

namespace Ploch.Common.Reflection
{
    public class PropertyNotFoundException: Exception
    {

        public PropertyNotFoundException(string propertyName): base(GetDefaultMessage(propertyName))
        {
            PropertyName = propertyName;
        }

        public PropertyNotFoundException(string propertyName, string message, Exception innerException): base(message, innerException)
        {
            PropertyName = propertyName;
        }

        protected PropertyNotFoundException(SerializationInfo info, StreamingContext context): base(info, context)
        { }

        public string PropertyName { get; }

        private static string GetDefaultMessage(string propertyName)
        {
            return $"Property {propertyName} was not found.";
        }
    }
}