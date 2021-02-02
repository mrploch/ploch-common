using System;
using System.Linq;
using Ardalis.GuardClauses;

namespace Ploch.Common.Reflection
{
    public static class TypeExtensions
    {
        public static bool IsImplementing(this Type type, Type interfaceType)
        {
            Guard.Against.Null(interfaceType, nameof(interfaceType));
            Guard.Against.Null(type, nameof(type));

            return type.GetInterfaces()
                       .Any(i => i == interfaceType
                                 || i.IsGenericType
                                 && i.GetGenericTypeDefinition() == interfaceType);
        }
    }
}