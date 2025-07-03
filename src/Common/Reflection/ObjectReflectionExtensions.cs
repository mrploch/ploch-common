using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Collections;

namespace Ploch.Common.Reflection;

public static class TypeHelper
{
    public static IDictionary<string, object?> GetStaticFieldValues<TType>(BindingFlags bindingFlags = BindingFlags.Public) =>
        ObjectReflectionExtensions.GetFieldValues<TType>(default, bindingFlags | BindingFlags.Static);
}

/// <summary>
///     Provides extension methods for working with object reflection.
/// </summary>
public static class ObjectReflectionExtensions
{
    //TODO: Move to a new MemberValueProviders object, because the GetStaticField/Property etc. will not extend object.

    /// <summary>
    ///     Gets the value of a field by name including non-public, instance and static members.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fieldName">The field name.</param>
    /// <returns>The field value if found or null.</returns>
    public static object? GetFieldValue(this object obj, string fieldName)
    {
        var fieldInfo = obj.NotNull(nameof(obj))
                           .GetType()
                           .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        return fieldInfo?.GetValue(obj);
    }

    /// <summary>
    ///     Gets the value of a field by name including non-public, instance and static members.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fieldName">The field name.</param>
    /// <typeparam name="TValue">The object type.</typeparam>
    /// <returns>The field value if found or default.</returns>
    public static TValue? GetFieldValue<TValue>(this object obj, string fieldName) => (TValue?)obj.GetFieldValue(fieldName);

    /// <summary>
    ///     Retrieves the values of all fields from an object or type.
    /// </summary>
    /// <typeparam name="TType">The type of the object whose fields are to be retrieved.</typeparam>
    /// <param name="obj">
    ///     The object from which to retrieve field values. If null, retrieves only static fields from the type.
    /// </param>
    /// <param name="bindingFlags">
    ///     A combination of <see cref="BindingFlags" /> that specifies how the search for fields is conducted.
    ///     By default, the search is performed for instance and public fields.
    /// </param>
    /// <returns>
    ///     A dictionary where each key is the field name, and each value is the corresponding field value.
    /// </returns>
    public static IDictionary<string, object?> GetFieldValues<TType>(this TType? obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
    {
        if (obj is null)
        {
            bindingFlags &= ~BindingFlags.Instance;
        }

        return obj.GetMemberValues(bindingFlags, MemberTypes.Field);
    }

    /// <summary>
    ///     Retrieves all static field values of a specified type, including non-public members.
    /// </summary>
    /// <typeparam name="TType">The type whose static fields are to be retrieved.</typeparam>
    /// <param name="bindingFlags">
    ///     A combination of <see cref="BindingFlags" /> that specifies how the search for fields is conducted.
    ///     By default, the search is performed for public fields.
    /// </param>
    /// <returns>
    ///     A dictionary where each key is the field name, and each value is the corresponding field value.
    /// </returns>
    /// <summary>
    ///     Retrieves the values of static members (fields and/or properties) from a specified type.
    /// </summary>
    /// <param name="memberTypes">
    ///     The types of members to retrieve. By default, both fields and properties are included.
    /// </param>
    /// <returns>
    ///     A dictionary where each key is the member name, and each value is the corresponding member value.
    /// </returns>
    public static IDictionary<string, object?> GetMemberValues<TType>(BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public,
                                                                      MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property)
    {
        bindingFlags &= ~BindingFlags.Instance;

        return GetMemberValues<TType>(default, bindingFlags | BindingFlags.Static, memberTypes);
    }

    /// <summary>
    ///     Retrieves the values of specified member types (fields and/or properties) from an object or type.
    /// </summary>
    /// <param name="obj">
    ///     The object from which to retrieve member values. If null, retrieves only static members from the type.
    /// </param>
    /// <param name="bindingFlags">
    ///     The binding flags that control the visibility and scope of members to retrieve. Defaults to
    ///     <see cref="BindingFlags.Instance" /> | <see cref="BindingFlags.Public" />.
    /// </param>
    /// <param name="memberTypes">
    ///     The types of members to retrieve, such as fields and/or properties. Defaults to
    ///     <see cref="MemberTypes.Field" /> | <see cref="MemberTypes.Property" />.
    /// </param>
    /// <param name="reflectedType">
    ///     The type to use for reflection instead of deriving the type from the object. If null, uses the object's runtime type.
    /// </param>
    /// <returns>
    ///     A dictionary where the keys are the names of the members and the values are the values of the members.
    /// </returns>
    public static IDictionary<string, object?> GetMemberValues<TType>(this TType? obj,
                                                                      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public,
                                                                      MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property,
                                                                      Type? reflectedType = null)
    {
        if (obj is null)
        {
            bindingFlags &= ~BindingFlags.Instance;
        }
        else
        {
            bindingFlags |= BindingFlags.DeclaredOnly;
        }

        var type = reflectedType ?? typeof(TType);

        if (type == typeof(object))
        {
            type = obj?.GetType();
        }

        var memberValues = new Dictionary<string, object?>();

        if (type.RequiredNotNull("Type cannot be null here").BaseType is not null && type!.BaseType != typeof(object))
        {
            memberValues.AddMany(obj.GetMemberValues(bindingFlags, memberTypes, type.BaseType), DuplicateHandling.Overwrite);
        }

        bindingFlags &= ~BindingFlags.DeclaredOnly;
        var memberInfos = type.RequiredNotNull(nameof(type)).GetMembers(bindingFlags).Where(m => memberTypes.HasFlag(m.MemberType));
        foreach (var memberInfo in memberInfos)
        {
            if (memberInfo.IsIndexer())
            {
                memberValues[memberInfo.Name] = (PropertyInfo)memberInfo;

                continue;
            }

            if (memberInfo is PropertyInfo { CanRead: false })
            {
                continue;
            }

            memberValues[memberInfo.Name] = memberInfo.GetValue(obj);
        }

        return memberValues;
    }
}
