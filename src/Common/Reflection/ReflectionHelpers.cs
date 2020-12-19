using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ploch.Common.Reflection
{
    /// <summary>
    ///     Common reflection tasks convenience object extensions.
    /// </summary>
    public static class ReflectionHelpers
    {
        /// <summary>
        ///     Gets the public properties of specific type.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the properties to return.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="includeSubTypes">Include sub types of <typeparamref name="TPropertyType" /> in results.</param>
        /// <returns>List of public properties of specific type.(<see cref="PropertyInfo" />s). </returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj" /> is <see langword="null" /></exception>
        public static IEnumerable<PropertyInfo> GetProperties<TPropertyType>(this object obj, bool includeSubTypes = true)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var type = obj.GetType();

            var propertyType = typeof(TPropertyType);

            return type.GetTypeInfo()
                       .GetProperties()
                       .Where(
                           pi => includeSubTypes
                               ? propertyType.GetTypeInfo().IsAssignableFrom(pi.PropertyType)
                               : pi.PropertyType == propertyType);
        }

        /// <summary>
        ///     Sets the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="AmbiguousMatchException">More than one property is found with the specified name. See Remarks.</exception>
        /// <exception cref="TargetException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch
        ///     <see cref="T:System.Exception" /> instead. The type of <paramref name="obj" /> does not match the target type, or a
        ///     property is an instance property but <paramref name="obj" /> is null.
        /// </exception>
        /// <exception cref="MethodAccessException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch the
        ///     base class exception, <see cref="T:System.MemberAccessException" />, instead. There was an illegal attempt to
        ///     access a private or protected method inside a class.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        ///     An error occurred while setting the property value. The
        ///     <see cref="P:System.Exception.InnerException" /> property indicates the reason for the error.
        /// </exception>
        public static void SetPropertyValue<T>(this T obj, string propertyName, object value)
        {
            typeof(T).GetTypeInfo().GetProperty(propertyName)?.SetValue(obj, value);
        }

        /// <summary>
        ///     Gets the property value.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Property value.</returns>
        /// <exception cref="AmbiguousMatchException">More than one property is found with the specified name. See Remarks.</exception>
        public static object GetPropertyValue<T>(this T obj, string propertyName)
        {
            return typeof(T).GetTypeInfo().GetProperty(propertyName)?.GetValue(obj);
        }

        /// <exception cref="AmbiguousMatchException">More than one property is found with the specified name. See Remarks.</exception>
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetTypeInfo().GetProperty(propertyName) != null;
        }
    }
}