using System;
using System.Globalization;
using System.Reflection;

namespace Ploch.Common.Linq;

public interface IOwnedPropertyInfo
{
    object GetValue();

    void SetValue(object value);
}

public interface IOwnedPropertyInfo<TType, TProperty>
{
    TType Owner { get; }

     TProperty GetValue();

    void SetValue(TProperty? value);
}

public class OwnedPropertyInfo<TType, TProperty> : OwnedPropertyInfo, IOwnedPropertyInfo<TType, TProperty>, IOwnedPropertyInfo
{
    public OwnedPropertyInfo(PropertyInfo propertyInfo, TType owner) : base(propertyInfo, owner)
    { }

    public TType Owner => (TType)base.Owner;

    public new TProperty GetValue()
    {
        return (TProperty)base.GetValue();
    }

    void IOwnedPropertyInfo.SetValue(object value)
    {
        SetValue(value);
    }

    public void SetValue(TProperty? value)
    {
        SetValue((object?)value);
    }

    object IOwnedPropertyInfo.GetValue()
    {
        return base.GetValue();
    }
}

public class OwnedPropertyInfo : PropertyInfo
{
    public OwnedPropertyInfo(PropertyInfo propertyInfo, object owner)
    {
        Owner = owner;
        PropertyInfo = propertyInfo;
    }

    public object Owner { get; }

    public PropertyInfo PropertyInfo { get; }

    public override Type DeclaringType => PropertyInfo.DeclaringType;

    public override string Name => PropertyInfo.Name;

    public override Type ReflectedType => PropertyInfo.ReflectedType;

    public override PropertyAttributes Attributes => PropertyInfo.Attributes;

    public override bool CanRead => PropertyInfo.CanRead;

    public override bool CanWrite => PropertyInfo.CanWrite;

    public override Type PropertyType => PropertyInfo.PropertyType;

    public object GetValue()
    {
        return PropertyInfo.GetValue(Owner);
    }

    public void SetValue(object? value)
    {
        PropertyInfo.SetValue(Owner, value);
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
        return PropertyInfo.GetCustomAttributes(inherit);
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        return PropertyInfo.GetCustomAttributes(attributeType, inherit);
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        return PropertyInfo.IsDefined(attributeType, inherit);
    }

    public override MethodInfo[] GetAccessors(bool nonPublic)
    {
        return PropertyInfo.GetAccessors(nonPublic);
    }

    public override MethodInfo GetGetMethod(bool nonPublic)
    {
        return PropertyInfo.GetGetMethod(nonPublic);
    }

    public override ParameterInfo[] GetIndexParameters()
    {
        return PropertyInfo.GetIndexParameters();
    }

    public override MethodInfo GetSetMethod(bool nonPublic)
    {
        return PropertyInfo.GetSetMethod(nonPublic);
    }

    public object GetValue(BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
        return GetValue(Owner, invokeAttr, binder, index, culture);
    }

    public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
        return PropertyInfo.GetValue(obj, invokeAttr, binder, index, culture);
    }

    public object GetValue(object[] index)
    {
        return PropertyInfo.GetValue(Owner, index);
    }

    public void SetValue(object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
        SetValue(Owner, value, invokeAttr, binder, index, culture);
    }

    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
        PropertyInfo.SetValue(obj, value, invokeAttr, binder, index, culture);
    }
}