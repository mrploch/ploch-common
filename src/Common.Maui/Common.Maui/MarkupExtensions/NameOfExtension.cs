using System.Reflection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Ploch.Common.Maui.MarkupExtensions;
[ContentProperty(nameof(Member))]
public class NameOfExtension : IMarkupExtension
{
    public Type? Type { get; set; }

    public string? Member { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        if (Type == null)
        {
            throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]} Member=[propertyName]");
        }

        if (Member == null)
        {
            return Type.Name;
        }

        if (string.IsNullOrEmpty(Member) || Member.Contains("."))
        {
            throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]} Member=[propertyName]");
        }

        var pinfo = Type.GetRuntimeProperties().FirstOrDefault(pi => pi.Name == Member);
        var finfo = Type.GetRuntimeFields().FirstOrDefault(fi => fi.Name == Member);

        if (pinfo == null && finfo == null)
        {
            throw new ArgumentException($"No property or field found for {Member} in {Type}");
        }

        return pinfo.Name ?? finfo.Name;
    }
}
