namespace Ploch.Common.ObjectBuilder;

/// <summary>
///     Provides a mechanism for converting management object types to specific target types.
///     This interface defines two methods for determining if a specific type can be handled
///     and for mapping values to a target type, along with an order property for prioritization.
/// </summary>
public interface IObjectTypeConverter
{
    /// <summary>
    ///     Represents the relative priority of the type converter when multiple converters
    ///     are used.
    /// </summary>
    /// <remarks>
    ///     A lower number indicates a higher priority. Type converters with
    ///     lower `Order` values are evaluated before those with higher values.
    ///     This property allows the framework to determine the sequence in which
    ///     type converters are applied when resolving or mapping management object types.
    ///     The `Order` property is typically implemented as a constant or readonly value
    ///     in derived classes, ensuring a consistent priority for each converter implementation.
    /// </remarks>
    int Order { get; }

    /// <summary>
    ///     Determines whether the current converter can handle the specified value and target type.
    /// </summary>
    /// <param name="value">The value to be converted, which can be null.</param>
    /// <param name="targetType">The target type to which the value needs to be converted.</param>
    /// <returns>
    ///     True if the converter can handle the conversion from the given value to the target type; otherwise, false.
    /// </returns>
    bool CanHandle(object? value, Type targetType);

    /// <summary>
    ///     Maps the specified value to the given target type using the conversion logic implemented by the current converter.
    /// </summary>
    /// <param name="value">The value to be converted, which can be null.</param>
    /// <param name="targetType">The target type to which the value needs to be converted.</param>
    /// <returns>
    ///     The converted value, or null if the conversion cannot be performed or the input value is null.
    /// </returns>
    object? ConvertValue(object? value, Type targetType);
}
