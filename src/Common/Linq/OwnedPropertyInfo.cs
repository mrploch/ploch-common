using System.Reflection;
using Ploch.Common.ArgumentChecking;

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
        propertyInfo.NotNull(nameof(propertyInfo));
        owner.NotNull(nameof(owner));

        PropertyInfo = propertyInfo;
        Owner = owner;
    }

    /// <inheritdoc />
    public object Owner { get; }

    /// <inheritdoc />
    public PropertyInfo PropertyInfo { get; }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public object? GetValue() => PropertyInfo.GetValue(Owner);

    /// <inheritdoc />
    public object? GetValue(object[] index) => PropertyInfo.GetValue(Owner, index);
}

/// <inheritdoc cref="IOwnedPropertyInfo{TProperty}" />
public class OwnedPropertyInfo<TProperty>(PropertyInfo propertyInfo, object owner) : OwnedPropertyInfo(propertyInfo, owner!), IOwnedPropertyInfo<TProperty>
{
    /// <inheritdoc />
    public new TProperty? GetValue() => (TProperty?)base.GetValue();

    /// <inheritdoc />
    public new TProperty? GetValue(object[] index) => (TProperty?)base.GetValue(index);

    /// <inheritdoc />
    public void SetValue(TProperty? value)
    {
        SetValue((object?)value);
    }

    /// <inheritdoc />
    public void SetValue(TProperty? value, object[] index)
    {
        base.SetValue(value, index);
    }

    /// <inheritdoc />
    void IOwnedPropertyInfo.SetValue(object? value)
    {
        SetValue(value);
    }

    /// <inheritdoc />
    void IOwnedPropertyInfo.SetValue(object? value, object[] index)
    {
        SetValue(value, index);
    }

    /// <inheritdoc />
    object? IOwnedPropertyInfo.GetValue() => base.GetValue();
}

/// <inheritdoc cref="IOwnedPropertyInfo{TType, TProperty}" />
/// <summary>
///     Initializes a new instance of the <see cref="OwnedPropertyInfo{TType,TProperty}" /> class.
/// </summary>
/// <param name="propertyInfo">The property info delegate.</param>
/// <param name="owner">The object that owns the property.</param>
public class OwnedPropertyInfo<TType, TProperty>(PropertyInfo propertyInfo, TType owner)
    : OwnedPropertyInfo<TProperty>(propertyInfo, owner!), IOwnedPropertyInfo<TType, TProperty>

{
    /// <inheritdoc />
    public new TType Owner => (TType)base.Owner;
}
