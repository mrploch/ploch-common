using System.Globalization;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <inheritdoc cref="IManagementObjectTypeConverter" />
public class DefaultManagementObjectTypeConverter : IManagementObjectTypeConverter
{
    /// <summary>
    ///     Represents the order of the mapper within the processing chain.
    /// </summary>
    /// <remarks>
    ///     This constant is exposed to allow other converters to specify their order of execution
    ///     based on other converters.
    /// </remarks>
    public const int MapperOrder = int.MaxValue;

    /// <inheritdoc cref="IManagementObjectTypeConverter.Order" />
    public int Order => MapperOrder;

    /// <summary>
    ///     Determines whether the converter can handle the specified value and target type.
    /// </summary>
    /// <param name="value">The object whose suitability for conversion is to be checked. This can be null.</param>
    /// <param name="targetType">The target type to which the value might be converted.</param>
    /// <returns>True if the converter can handle the conversion; otherwise, false.</returns>
    public bool CanHandle(object? value, Type targetType) => true;

    /// <summary>
    ///     Maps the given value to the specified target type using type conversion.
    /// </summary>
    /// <param name="value">The object to be converted. This can be null.</param>
    /// <param name="targetType">The target type to which the value should be converted.</param>
    /// <returns>The converted object, or null if the input value is null.</returns>
    /// <exception cref="TypeConversionException">
    ///     Thrown when a conversion error occurs, such as an invalid format, invalid cast, or overflow during type conversion.
    /// </exception>
    public object? MapValue(object? value, Type targetType)
    {
        if (value is null)
        {
            return null;
        }

        if (targetType == typeof(string))
        {
            return value.ToString();
        }

        try
        {
            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (FormatException ex)
        {
            throw new TypeConversionException(value, targetType, ex);
        }
        catch (InvalidCastException ex)
        {
            throw new TypeConversionException(value, targetType, ex);
        }
        catch (OverflowException ex)
        {
            throw new TypeConversionException(value, targetType, ex);
        }
    }
}
