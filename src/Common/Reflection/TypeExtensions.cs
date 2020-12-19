using System;
using System.Linq;
using Validation;

namespace Ploch.Common.Reflection
{
    public static class TypeExtensions
    {
        public static bool IsImplementing(this Type type, Type interfaceType)
        {
            Requires.NotNull(interfaceType, nameof(interfaceType));
            Requires.NotNull(type, nameof(type));

            return type.GetInterfaces()
                       .Any(i => i == interfaceType
                                 || i.IsGenericType
                                 && i.GetGenericTypeDefinition() == interfaceType);
        }
    }
}