namespace Ploch.Common.TypeConversion;

/// <summary>
///     Represents an interface for converting values to a specific target type.
///     This interface is specialized to provide conversion logic for a single target type.
///     Implementations of this interface should aim to provide a robust mechanism for converting
///     a variety of source input formats to a designated target type while handling any potential edge cases.
/// </summary>
/// <typeparam name="TTargetType">
///     The data type to which the conversion is targeted. Implementations of the interface will use this
///     as the conversion's destination type.
/// </typeparam>
/// <remarks>
///     <para>
///         The <see cref="ITargetTypeConverter{TTargetType}" /> interface extends the <see cref="ITypeConverter" /> interface,
///         providing a focused API for converting values specifically to the type <typeparamref name="TTargetType" />.
///     </para>
///     <para>
///         The interface also provides additional abstraction, allowing consumers to handle target type-specific conversions
///         without needing to directly specify the source type for the conversion.
///     </para>
/// </remarks>
public interface ITargetTypeConverter<out TTargetType> : ITypeConverter
{
    /// <summary>
    ///     Converts the specified value to the target type.
    /// </summary>
    /// <param name="value">The value to be converted. This parameter can be null or any type that is compatible with the target type.</param>
    /// <returns>The converted value as the target type, or null if the value cannot be converted.</returns>
    TTargetType? ConvertValueToTargetType(object? value);
}
