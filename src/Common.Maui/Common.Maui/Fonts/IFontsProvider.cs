using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Ploch.Common.Maui.Fonts;

public interface IFontsProvider
{
    ReadOnlyDictionary<string, FontMapping> GetFonts();

    FontMapping? GetFont(string fontFamily);

    bool TryGetFont(string fontFamily, [NotNullWhen(true)] out FontMapping? fontMapping);

    string? GetGlyph(string fontFamily, string glyphName);
}
