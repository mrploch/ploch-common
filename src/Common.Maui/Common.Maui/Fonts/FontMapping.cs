using System.Reflection;

namespace Ploch.Common.Maui.Fonts;

/// <summary>
///     Initializes a new instance of the <see cref="IconFontMapping{TFontIconNames}" /> class.
/// </summary>
/// <remarks>
///     The <see cref="IconFontMapping{TFontIconNames}" /> class is a specialized record derived from <see cref="FontMapping" />.
///     It represents a mapping between a font family and its file, including predefined constants for font icons.
///     This generic record is designed for handling icon fonts by incorporating a type that defines a set of constant glyphs.
/// </remarks>
/// <typeparam name="TFontIconNames">
///     The type that provides constants representing font glyphs.
///     It is typically an enumeration or static class containing names and values for the glyphs available in the font.
/// </typeparam>
/// <param name="fontFamily">The name of the font family.</param>
/// <param name="fontFileName">The name of the font file.</param>
/// <param name="FontIcons">
///     The <typeparamref name="TFontIconNames" /> instance representing constants for font icons.
/// </param>
public record IconFontMapping<TFontIconNames>(string fontFamily, string fontFileName, TFontIconNames FontIcons)
    : FontMapping(fontFamily, fontFileName, true, typeof(TFontIconNames));

/// <summary>
///     Initializes a new instance of the <see cref="FontMapping" /> class.
/// </summary>
/// <remarks>
///     The FontMapping class is a record that represents a mapping between a font family,
///     its file name, and optional constants for glyphs. It provides functionality to retrieve glyph
///     values by name.
/// </remarks>
/// <param name="FontFamily">The name of the font family.</param>
/// <param name="FontFileName">The name of the font file.</param>
/// <param name="IsImageFont">Indicates whether the font is an image font.</param>
/// <param name="FontConstantsType">The type containing constants for font glyphs (optional).</param>
public record FontMapping(string FontFamily, string FontFileName, bool IsImageFont = false, Type? FontConstantsType = null)
{
    private readonly IDictionary<string, string> _cachedGlyphs = new Dictionary<string, string>();
    private bool _allGlyphsCached;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontMapping" /> class with the specified font family,
    ///     font file name, and a type containing font constants.
    /// </summary>
    /// <param name="fontFamily">The name of the font family.</param>
    /// <param name="fontFileName">The name of the font file.</param>
    /// <param name="fontConstantsType">The type containing constants for font glyphs.</param>
    public FontMapping(string fontFamily, string fontFileName, Type? fontConstantsType) : this(fontFamily, fontFileName, true, fontConstantsType)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontMapping" /> class with the specified font family and font file
    ///     name.
    /// </summary>
    /// <param name="fontFamily">The name of the font family.</param>
    /// <param name="fontFileName">The name of the font file.</param>
    public FontMapping(string fontFamily, string fontFileName) : this(fontFamily, fontFileName, false)
    { }

    /// <summary>
    ///     Retrieves the glyph value associated with the specified glyph name.
    /// </summary>
    /// <param name="glyphName">The name of the glyph.</param>
    /// <returns>
    ///     The glyph value as a string, or <c>null</c> if the glyph is not found or the font constants type is not specified.
    /// </returns>
    public string? GetGlyphValue(string glyphName)
    {
        if (FontConstantsType is null)
        {
            return null;
        }

        if (_cachedGlyphs.TryGetValue(glyphName, out var cachedValue))
        {
            return cachedValue;
        }

        var field = FontConstantsType.GetField(glyphName, BindingFlags.Static | BindingFlags.Public);

        if (field == null)
        {
            return null;
        }

        var value = field.GetValue(null)?.ToString();

        if (value == null)
        {
            return null;
        }

        _cachedGlyphs.Add(glyphName, value);

        return value;
    }

    public IReadOnlyDictionary<string, string>? GetAllGlypns()
    {
        if (FontConstantsType is null)
        {
            return null;
        }

        if (_allGlyphsCached)
        {
            return _cachedGlyphs.AsReadOnly();
        }

        var constantFields = FontConstantsType.GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in constantFields)
        {
            var value = field.GetRawConstantValue();

            if (value == null)
            {
                continue;
            }

            var stringValue = value.ToString();

            if (stringValue == null)
            {
                continue;
            }

            _cachedGlyphs.TryAdd(field.Name, stringValue);
        }

        _allGlyphsCached = true;

        return _cachedGlyphs.AsReadOnly();
    }
}
