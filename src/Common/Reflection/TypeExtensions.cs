using System;
using System.Linq;
using Dawn;

namespace Ploch.Common.Reflection
{
    public static class TypeExtensions
    {
        public static bool IsImplementing(this Type type, Type interfaceType)
        {
            Guard.Argument(interfaceType, nameof(interfaceType)).NotNull();
            Guard.Argument(type, nameof(type)).NotNull();

            return type.GetInterfaces()
                       .Any(i => i == interfaceType
                                 || (i.IsGenericType
                                     && i.GetGenericTypeDefinition() == interfaceType));
        }
    }
}