using System.Reflection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Ploch.Common.Maui.MarkupExtensions;

/// <summary>
///     XAML markup extension that resolves the name of a property or field on a type at parse time,
///     giving XAML the equivalent of C#'s <see langword="nameof" /> operator.
/// </summary>
/// <example>
///     <code>
/// &lt;Label Text="{x:NameOf Type={x:Type local:Person}, Member=FirstName}" /&gt;
///     </code>
/// </example>
[ContentProperty(nameof(Member))]
public class NameOfExtension : IMarkupExtension
{
    /// <summary>
    ///     Gets or sets the type that declares the member to resolve.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    ///     Gets or sets the name of the property or field to resolve. When omitted, the name of
    ///     <see cref="Type" /> itself is returned.
    /// </summary>
    public string? Member { get; set; }

    /// <summary>
    ///     Resolves the configured member and returns its name.
    /// </summary>
    /// <param name="serviceProvider">The service provider supplied by the XAML parser.</param>
    /// <returns>
    ///     The name of the member named by <see cref="Member" />, or the name of <see cref="Type" />
    ///     when <see cref="Member" /> is <see langword="null" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="serviceProvider" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <see cref="Type" /> is <see langword="null" />, when <see cref="Member" /> is empty
    ///     or contains a dot, or when no property or field of that name exists on <see cref="Type" />.
    /// </exception>
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        if (Type == null)
        {
            throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]} Member=[propertyName]");
        }

        if (Member == null)
        {
            return Type.Name;
        }

        if (string.IsNullOrEmpty(Member) || Member.Contains('.'))
        {
            throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]} Member=[propertyName]");
        }

        var pinfo = Type.GetRuntimeProperties().FirstOrDefault(pi => pi.Name == Member);
        if (pinfo != null)
        {
            return pinfo.Name;
        }

        var finfo = Type.GetRuntimeFields().FirstOrDefault(fi => fi.Name == Member);
        if (finfo != null)
        {
            return finfo.Name;
        }

        throw new ArgumentException($"No property or field found for {Member} in {Type}");
    }
}
