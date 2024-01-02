using System;
using System.Globalization;
using System.Reflection;

namespace Ploch.Common.Linq;

/// <inheritdoc cref="IOwnedPropertyInfo" />
public abstract class OwnedPropertyInfo : IOwnedPropertyInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OwnedPropertyInfo" /> class.
    /// </summary>
    /// <param name="propertyInfo">The property info delegate.</param>
    /// <param name="owner">The object that owns the property.</param>
    protected OwnedPropertyInfo(PropertyInfo propertyInfo, object owner)
    {
        Owner = owner;
        PropertyInfo = propertyInfo;
    }

    /// <inheritdoc cref="IOwnedPropertyInfo.Owner" />
    public object Owner { get; }

    /// <summary>
    ///     Gets an underlying <see cref="PropertyInfo" />.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <inheritdoc cref="PropertyInfo.Name" />
    public string Name => PropertyInfo.Name;

    public object? GetValue()
    {
        return PropertyInfo.GetValue(Owner);
    }

    /// <inheritdoc />
    public void SetValue(object? value)
    {
        PropertyInfo.SetValue(Owner, value);
    }

    /// <inheritdoc />
    public void SetValue(object? value, object[] index)
    {
        PropertyInfo.SetValue(Owner, value, index);
    }

    /// <inheritdoc cref="PropertyInfo.GetCustomAttributes(bool)" />
    public object[] GetCustomAttributes(bool inherit)
    {
        return PropertyInfo.GetCustomAttributes(inherit);
    }

    /// <inheritdoc cref="PropertyInfo.GetCustomAttributes(Type,bool)" />
    public object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        return PropertyInfo.GetCustomAttributes(attributeType, inherit);
    }

    /// <inheritdoc cref="PropertyInfo.IsDefined" />
    public bool IsDefined(Type attributeType, bool inherit)
    {
        return PropertyInfo.IsDefined(attributeType, inherit);
    }

    /// <inheritdoc cref="PropertyInfo.GetAccessors()" />
    public MethodInfo[] GetAccessors(bool nonPublic)
    {
        return PropertyInfo.GetAccessors(nonPublic);
    }

    /// <inheritdoc cref="PropertyInfo.GetGetMethod()" />
    public MethodInfo GetGetMethod(bool nonPublic)
    {
        return PropertyInfo.GetGetMethod(nonPublic);
    }

    /// <inheritdoc cref="PropertyInfo.GetIndexParameters" />
    public ParameterInfo[] GetIndexParameters()
    {
        return PropertyInfo.GetIndexParameters();
    }

    /// <inheritdoc cref="PropertyInfo.GetSetMethod()" />
    public MethodInfo GetSetMethod(bool nonPublic)
    {
        return PropertyInfo.GetSetMethod(nonPublic);
    }

    // TODO:
    /// <summary>
    ///     Returns the property value of a property in the <see cref="Owner" /> object with a specified binding, index,
    ///     and culture-specific information.
    /// </summary>
    /// <param name="invokeAttr">
    ///     A bitwise combination of the following enumeration members that specify the invocation
    ///     attribute: <see langword="InvokeMethod" />, <see langword="CreateInstance" />, <see langword="Static" />,
    ///     <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, and
    ///     <see langword="SetProperty" />. You must specify a suitable invocation attribute. For example, to invoke a static
    ///     member, set the <see langword="Static" /> flag.
    /// </param>
    /// <param name="binder">
    ///     An object that enables the binding, coercion of argument types, invocation of members, and
    ///     retrieval of <see cref="MemberInfo" /> objects through reflection. If <paramref name="binder" /> is
    ///     <see langword="null" />, the default binder is used.
    /// </param>
    /// <param name="index">
    ///     Optional index values for indexed properties. This value should be <see langword="null" /> for
    ///     non-indexed properties.
    /// </param>
    /// <param name="culture">
    ///     The culture for which the resource is to be localized. If the resource is not localized for this
    ///     culture, the <see cref="CultureInfo.Parent" /> property will be called successively in search of a match. If this
    ///     value is <see langword="null" />, the culture-specific information is obtained from the
    ///     <see cref="CultureInfo.CurrentUICulture" /> property.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The <paramref name="index" /> array does not contain the type of arguments needed.
    ///     -or-
    ///     The property's <see langword="get" /> accessor is not found.
    /// </exception>
    /// <exception cref="TargetParameterCountException">
    ///     The number of parameters in <paramref name="index" /> does not match
    ///     the number of parameters the indexed property takes.
    /// </exception>
    /// <exception cref="MethodAccessException">
    ///     There was an illegal attempt to access a private or protected method inside a
    ///     class.
    /// </exception>
    /// <exception cref="TargetInvocationException">
    ///     An error occurred while retrieving the property value. For example, an
    ///     index value specified for an indexed property is out of range. The <see cref="Exception.InnerException" /> property
    ///     indicates the reason for the error.
    /// </exception>
    /// <returns>The property value.</returns>
    public object GetValue(BindingFlags invokeAttr, Binder? binder, object[]? index, CultureInfo? culture)
    {
        return GetValue(Owner, invokeAttr, binder, index, culture);
    }
    
    /// <inheritdoc cref="PropertyInfo.GetValue(object, BindingFlags, Binder, object[], CultureInfo)" />
    public override object GetValue(object obj, BindingFlags invokeAttr, Binder? binder, object[]? index, CultureInfo? culture)
    {
        return PropertyInfo.GetValue(obj, invokeAttr, binder, index, culture);
    }

    /// <inheritdoc cref="IOwnedPropertyInfo.GetValue(object[])" />
    public object GetValue(object[] index)
    {
        return PropertyInfo.GetValue(Owner, index);
    }

    /// <inheritdoc cref="System.Reflection.PropertyInfo.SetValue(object, object, BindingFlags, Binder, object[], CultureInfo)" />
    public override void SetValue(object obj, object? value, BindingFlags invokeAttr, Binder? binder, object[]? index, CultureInfo? culture)
    {
        PropertyInfo.SetValue(obj, value, invokeAttr, binder, index, culture);
    }
}

/// <inheritdoc cref="IOwnedPropertyInfo{TType, TProperty}" />
public class OwnedPropertyInfo<TType, TProperty> : OwnedPropertyInfo, IOwnedPropertyInfo<TType, TProperty>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OwnedPropertyInfo{TType,TProperty}" /> class.
    /// </summary>
    /// <param name="propertyInfo">The property info delegate.</param>
    /// <param name="owner">The object that owns the property.</param>
    public OwnedPropertyInfo(PropertyInfo propertyInfo, TType owner) : base(propertyInfo, owner)
    { }

    /// <inheritdoc cref="IOwnedPropertyInfo{TType,TProperty}.Owner" />
    public new TType Owner => (TType)base.Owner;

    /// <inheritdoc cref="IOwnedPropertyInfo{TType,TProperty}.GetValue()" />
    public new TProperty? GetValue()
    {
        return (TProperty?)base.GetValue();
    }

    /// <inheritdoc cref="IOwnedPropertyInfo{TType,TProperty}.GetValue(object[])" />
    public new TProperty? GetValue(object[] index)
    {
        return (TProperty?)base.GetValue(index);
    }

    /// <inheritdoc cref="IOwnedPropertyInfo.SetValue(object?)" />
    void IOwnedPropertyInfo.SetValue(object? value)
    {
        SetValue(value);
    }

    /// <inheritdoc cref="IOwnedPropertyInfo.SetValue(object?,object[])" />
    void IOwnedPropertyInfo.SetValue(object? value, object[] index)
    {
        SetValue(Owner, value, index);
    }

    /// <inheritdoc cref="IOwnedPropertyInfo{TType,TProperty}.SetValue(TProperty?)" />
    public void SetValue(TProperty? value)
    {
        SetValue((object?)value);
    }

    /// <inheritdoc />
    public void SetValue(TProperty? value, object[] index)
    {
        base.SetValue(value, index);
    }

    /// <inheritdoc cref="IOwnedPropertyInfo.GetValue" />
    object? IOwnedPropertyInfo.GetValue()
    {
        return base.GetValue();
    }

}