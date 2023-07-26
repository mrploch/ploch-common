using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Dawn;

namespace Ploch.Common.Reflection
{
    /// <summary>
    ///     Common reflection tasks convenience object extensions.
    /// </summary>
    public static class PropertyHelpers
    {
        /// <summary>
        ///     Gets the <see langword="public" /> properties of specific type.
        /// </summary>
        /// <typeparam name="TPropertyType">
        ///     The type of the properties to return.
        /// </typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="includeSubTypes">
        ///     Include sub types of <typeparamref name="TPropertyType" /> in
        ///     results.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="obj" /> is <see langword="null" />.
        /// </exception>
        /// <returns>
        ///     List of <see langword="public" /> properties of specific type.(
        ///     <see cref="PropertyInfo" /> s).
        /// </returns>
        public static IEnumerable<PropertyInfo> GetProperties<TPropertyType>(this object obj, bool includeSubTypes = true)
        {
            Guard.Argument(obj, nameof(obj)).NotNull();

            var type = obj.GetType();

            var propertyType = typeof(TPropertyType);

            return type.GetTypeInfo()
                       .GetProperties()
                       .Where(pi =>
                              {
                                  Debug.WriteLine(pi.Name);

                                  return includeSubTypes ? propertyType.GetTypeInfo().IsAssignableFrom(pi.PropertyType) : pi.PropertyType == propertyType;
                              });
        }

        /// <summary>
        ///     Sets the property.
        /// </summary>
        /// <typeparam name="T">The type of a property.</typeparam>
        /// <param name="obj">The object type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     More than one property is found with the specified name. See
        ///     Remarks.
        /// </exception>
        /// <exception cref="TargetException">
        ///     In the .NET for Windows Store apps or the Portable Class Library,
        ///     <see langword="catch" /> <see cref="Exception" /> instead. The type
        ///     of <paramref name="obj" /> does not match the target type, or a
        ///     property is an instance property but <paramref name="obj" /> is
        ///     null.
        /// </exception>
        /// <exception cref="MethodAccessException">
        ///     In the .NET for Windows Store apps or the Portable Class Library,
        ///     <see langword="catch" /> the base class exception,
        ///     <see cref="MemberAccessException" /> , instead. There was an illegal
        ///     attempt to access a <see langword="private" /> or
        ///     <see langword="protected" /> method inside a class.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        ///     An error occurred while setting the property value. The
        ///     <see cref="System.Exception.InnerException" /> property indicates
        ///     the reason for the error.
        /// </exception>
        /// <exception cref="PropertyNotFoundException">
        ///     If <paramref name="propertyName" /> property is not found.
        /// </exception>
        public static void SetPropertyValue<T>(this T obj, string propertyName, object value)
        {
            var propertyInfo = typeof(T).GetPropertyInfo(propertyName, true);

            if (propertyInfo == null)
            {
                throw new PropertyNotFoundException(propertyName);
            }

            propertyInfo.SetValue(obj, value);
        }

        /// <summary>
        ///     Gets the property value.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     More than one property is found with the specified name. See
        ///     Remarks.
        /// </exception>
        /// <exception cref="PropertyNotFoundException">
        ///     If <paramref name="propertyName" /> property is not found.
        /// </exception>
        /// <returns>
        ///     Property value.
        /// </returns>
        public static object GetPropertyValue<T>(this T obj, string propertyName)
        {
            var propertyInfo = typeof(T).GetPropertyInfo(propertyName, true);

            if (propertyInfo == null)
            {
                throw new PropertyNotFoundException(propertyName);
            }

            return propertyInfo.GetValue(obj);
        }

        /// <summary>
        ///     Determines whether the specified property name has property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     More than one property is found with the specified name. See
        ///     Remarks.
        /// </exception>
        /// <returns>
        ///     <c>true</c> if the specified property name has property; otherwise,
        ///     <c>false</c> .
        /// </returns>
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetPropertyInfo(propertyName, false) != null;
        }

        /// <summary>
        ///     Gets the property information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="throwIfNotFound">
        ///     if set to <c>true</c> throws
        ///     <see cref="PropertyNotFoundException" /> if property is not found.
        /// </param>
        /// <exception cref="PropertyNotFoundException">
        ///     if <paramref name="throwIfNotFound" /> is <c>true</c> and property
        ///     is not found.
        /// </exception>
        /// <returns>
        ///     Property information.
        /// </returns>
        public static PropertyInfo? GetPropertyInfo(this Type type, string propertyName, bool throwIfNotFound)
        {
            var propertyInfo = type.GetTypeInfo().GetProperty(propertyName);

            if (propertyInfo == null && throwIfNotFound)
            {
                throw new PropertyNotFoundException(propertyName);
            }

            return propertyInfo;
        }
    }
}