using System;
using System.ComponentModel.DataAnnotations;

namespace Ploch.Common.DataAnnotations;

/// <summary>
///     Validates that a date / time value is not null or default.
/// </summary>
/// <remarks>
///     <para>
///         Validates <see cref="DateTime" />, <see cref="DateTimeOffset" /> and <see cref="DateOnly" /> values,
///         checking that they do not have default value.
///         If any other type than the above types is provided, this class will return <c>invalid</c>.
///         This includes <see cref="TimeOnly" />.
///     </para>
/// </remarks>
public class RequiredNotDefaultDateAttribute : ValidationAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequiredNotDefaultDateAttribute" /> class.
    /// </summary>
    public RequiredNotDefaultDateAttribute() : base("The {0} field is required and must not be the default date or date time value.")
    { }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        return value switch
               {
                   DateTime dateTime => dateTime != default,
#if NET6_0_OR_GREATER
                   DateOnly dateOnly => dateOnly != default,
#endif
                   DateTimeOffset dateTimeOffset => dateTimeOffset != default,
                   _ => false
               };
    }
}