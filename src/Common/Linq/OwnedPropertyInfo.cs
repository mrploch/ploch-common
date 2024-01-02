using System.Reflection;
using Dawn;

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
        Guard.Argument(propertyInfo, nameof(propertyInfo)).NotNull();
        Guard.Argument(owner, nameof(owner)).NotNull();

        PropertyInfo = propertyInfo;
        Owner = owner;
    }

    /// <inheritdoc cref="IOwnedPropertyInfo.Owner" />
    public object Owner { get; }

    /// <summary>
    ///     Gets an underlying <see cref="PropertyInfo" />.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <inheritdoc cref="PropertyInfo.Name" />
    public string Name => PropertyInfo.Name;

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

    /// <summary>
    ///     Gets a value of the property.
    /// </summary>
    /// <returns>The property value.</returns>
    public object? GetValue()
    {
        return PropertyInfo.GetValue(Owner);
    }

    /// <inheritdoc cref="IOwnedPropertyInfo.GetValue(object[])" />
    public object GetValue(object[] index)
    {
        return PropertyInfo.GetValue(Owner, index);
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
    public OwnedPropertyInfo(PropertyInfo propertyInfo, TType owner) : base(propertyInfo, owner!)
    { }

    /// <inheritdoc cref="IOwnedPropertyInfo{TType,TProperty}.Owner" />
    public new TType Owner => (TType)base.Owner;

    /// <inheritdoc cref="IOwnedPropertyInfo{TType,TProperty}.GetValue()" />
    public new TProperty? GetValue()
    {
        var value = base.GetValue();

        return (TProperty?)value;
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
        SetValue(value, index);
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

    /// <inheritdoc cref="IOwnedPropertyInfo.GetValue()" />
    object? IOwnedPropertyInfo.GetValue()
    {
        return base.GetValue();
    }
}