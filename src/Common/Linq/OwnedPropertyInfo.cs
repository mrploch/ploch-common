using System.Diagnostics.CodeAnalysis;
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
public class OwnedPropertyInfo<TProperty>(PropertyInfo propertyInfo, object owner) : OwnedPropertyInfo(propertyInfo, owner), IOwnedPropertyInfo<TProperty> // skipcq: CS-R1103 - intentional generic specialization of the base type
{
    /// <inheritdoc />
    public new TProperty? GetValue() => (TProperty?)base.GetValue();

    /// <inheritdoc />
    public new TProperty? GetValue(object[] index) => (TProperty?)base.GetValue(index);

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1100:Do not prefix calls with base unless local implementation exists", Justification = "base. explicitly targets the base implementation; without it, overload resolution for a closed generic where TProperty is object would bind to this method and recurse.")]
    public void SetValue(TProperty? value)
    {
        base.SetValue((object?)value);
    }

    /// <inheritdoc />
    public void SetValue(TProperty? value, object[] index)
    {
        base.SetValue(value, index);
    }

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1100:Do not prefix calls with base unless local implementation exists", Justification = "base. explicitly targets the base implementation; without it, overload resolution for a closed generic where TProperty is object would bind to the derived SetValue(TProperty?) and recurse.")]
    void IOwnedPropertyInfo.SetValue(object? value)
    {
        base.SetValue(value);
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
#pragma warning disable PCA0001 // 'owner' is forwarded to the base constructor via the null-forgiving operator (the base performs the runtime null check); it is not mutated.
public class OwnedPropertyInfo<TType, TProperty>(PropertyInfo propertyInfo, TType owner)
    : OwnedPropertyInfo<TProperty>(propertyInfo, owner!), IOwnedPropertyInfo<TType, TProperty>
#pragma warning restore PCA0001
{
    /// <inheritdoc />
    public new TType Owner => (TType)base.Owner;
}
