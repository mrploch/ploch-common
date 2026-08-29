namespace Ploch.Common.Maui.Fonts;

public interface IFontsBuilder
{
    IFontsBuilder AddFont(string fontFileName, string fontFamily, Action<FontMapping> fontMappingSetter);

    IFontsBuilder AddFont(string fontFileName, string fontFamily, Type? fontGlyphConstantsType, Action<FontMapping> fontMappingSetter);
}
