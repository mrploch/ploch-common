namespace Ploch.Common.TypeConversion;

/// <summary>
///     Defines an interface for type converters that are specifically targeted for converting from a
///     particular source type, while implementing the general <see cref="ITypeConverter" /> interface.
/// </summary>
/// <typeparam name="TSourceType">
///     The source type that this converter specializes in handling for the conversion process. This is the type from
///     which conversions are initiated.
/// </typeparam>
/// <remarks>
///     The <c>ISourceTypeConverter</c> interface serves as an abstraction for implementing type conversion
///     logic for specific source types. Implementing classes generally build on top of this interface by specializing
///     the source type and adding custom logic to convert the source type to various target types supported by
///     <see cref="ITypeConverter.ConvertValue" />.
///     Converters implementing this interface typically work in collaboration with other converters or frameworks
///     for custom type conversion flows.
/// </remarks>
/// <example>
///     This interface may be implemented for creating type converters that convert a single source type
///     (e.g., `string`, specific enum type, or custom object) into multiple possible target types such as
///     primitives, objects, or other specific domain types.
///     Custom implementations of <see cref="ISourceTypeConverter{TSourceType}" /> would allow for clearly
///     scoped and optimized conversion logic tailored to the defined source type.
/// </example>
public interface ISourceTypeConverter<TSourceType> : ITypeConverter
{ }
