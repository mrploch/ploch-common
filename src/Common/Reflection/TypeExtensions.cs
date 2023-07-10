using System;
using System.Linq;
using Dawn;

namespace Ploch.Common.Reflection
{
    /// <summary>
    ///     <see cref="System.Type" /> extension methods.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Checks if the type provided is implementing the specified interface.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">The type of interface.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="type" /> is implementing <paramref name="interfaceType" />, false
        ///     otherwise.
        /// </returns>
        public static bool IsImplementing(this Type type, Type interfaceType)
        {
            Guard.Argument(interfaceType, nameof(interfaceType)).NotNull();
            Guard.Argument(type, nameof(type)).NotNull();

            return type.GetInterfaces().Any(i => i == interfaceType || (i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType));
        }
    }
}