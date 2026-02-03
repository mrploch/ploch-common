using Ploch.Common.Maui.Fonts;

namespace Ploch.Lists.UI.MauiUI.Common.Fonts;
public interface IFontsBuilder
{
    IFontsBuilder AddFont(string fontFileName, string fontFamily, Action<FontMapping> fontMappingSetter);

    IFontsBuilder AddFont(string fontFileName, string fontFamily, Type? fontGlyphConstantsType, Action<FontMapping> fontMappingSetter);
}
