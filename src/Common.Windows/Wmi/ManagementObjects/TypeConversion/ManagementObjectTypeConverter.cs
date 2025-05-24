using Ploch.Common.Reflection;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <inheritdoc />
public abstract class ManagementObjectTypeConverter<TSourceType> : IManagementObjectTypeConverter
{
    protected ManagementObjectTypeConverter(params IEnumerable<Type> supportedTargetTypes) => SupportedTargetTypes = new HashSet<Type>(supportedTargetTypes);

    /// <summary>
    ///     Represents the processing order or priority for the type converter during type conversion operations.
    /// </summary>
    /// <remarks>
    ///     The <c>Order</c> property determines the relative precedence of the type converter when multiple converters
    ///     are available. Lower values indicate higher priority. This property is typically used by type conversion systems
    ///     to select the most suitable converter based on defined priorities.
    /// </remarks>
    public abstract int Order { get; }

    /// <summary>
    ///     Determines whether this converter can handle the conversion of the specified value to the target type.
    /// </summary>
    /// <param name="value">The value to be converted. Can be null.</param>
    /// <param name="targetType">The type to which the value should be converted.</param>
    /// <returns>
    ///     <c>true</c> if this converter can handle the conversion; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     This method performs the following checks:
    ///     1. Ensures the value is of type TSourceType or null.
    ///     2. Checks if the value is null and the target type is a non-nullable value type.
    ///     3. Verifies if the target type is supported by this converter.
    ///     4. Calls the type-specific CanHandle method for additional checks.
    /// </remarks>
    public bool CanHandle(object? value, Type targetType)
    {
        if (!(value is TSourceType || value is null))
        {
            return false;
        }

        if (value is null && targetType.IsValueType && !targetType.IsNullable())
        {
            return false;
        }

        if (!IsTargetTypeSupported(targetType))
        {
            return false;
        }

        return CanHandle((TSourceType)value, targetType);
    }

    /// <summary>
    ///     Converts the specified value to the target type.
    /// </summary>
    /// <param name="value">The value to be converted. Can be null.</param>
    /// <param name="targetType">The type to which the value should be converted.</param>
    /// <returns>
    ///     The converted value of the specified target type, or null if the conversion is unsuccessful or the value is null.
    /// </returns>
    public virtual object? MapValue(object? value, Type targetType) => MapValue((TSourceType?)value, targetType);

    /// <summary>
    ///     Represents a collection of supported target types that this type converter can handle for type conversion operations.
    /// </summary>
    /// <remarks>
    ///     This property provides a set of target <see cref="Type" /> objects that the converter supports for mapping source values.
    ///     It is typically initialized in the constructor of the derived type converter to ensure the converter can determine if
    ///     a given target type is compatible with its mapping functionality.
    /// </remarks>
    protected ISet<Type> SupportedTargetTypes { get; }

    protected virtual bool IsTargetTypeSupported(Type targetType) => SupportedTargetTypes.Contains(targetType);

    /// <summary>
    ///     Determines whether the specified value can be converted to the given target type by this converter.
    /// </summary>
    /// <param name="value">The value to be checked. Can be null.</param>
    /// <param name="targetType">The type to which the value should be converted.</param>
    /// <returns>
    ///     <c>true</c> if the value can be handled and converted to the specified target type; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     This method performs a series of checks to determine if the value and target type meet the criteria for conversion
    ///     supported by the specific implementation of this converter.
    /// </remarks>
    protected virtual bool CanHandle(TSourceType? value, Type targetType) => true;

    /// <summary>
    ///     Converts the given value of the source type to the specified target type.
    /// </summary>
    /// <param name="value">The value to be converted. Can be null.</param>
    /// <param name="targetType">The type to which the value should be converted.</param>
    /// <returns>
    ///     The converted value as an instance of the specified target type, or null if the conversion fails or the input value is null.
    /// </returns>
    protected abstract object? MapValue(TSourceType? value, Type targetType);

    private static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;
}
