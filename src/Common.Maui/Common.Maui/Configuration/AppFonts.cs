using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Ploch.Common.Maui.Fonts;
using Ploch.Lists.UI.MauiUI.Common.Fonts;

namespace Ploch.Common.Maui.Configuration;

public class AppFonts : IFontsBuilder, IFontsProvider
{
    private readonly Dictionary<string, FontMapping> _fonts = new();

    public AppFonts()
    {
        AddFont("MaterialIcons-Regular.ttf", nameof(MaterialIconsRegular), typeof(MaterialDesignRegularFont), mapping => MaterialIconsRegular = mapping)
            .AddFont("materialdesignicons-webfont.ttf", nameof(MaterialDesignIcons), typeof(MaterialDesignWebFont), mapping => MaterialDesignIcons = mapping)
            .AddFont("FontAwesome6FreeRegular.otf", nameof(FontAwesomeRegular), typeof(FontAwesomeRegular), mapping => FontAwesomeRegular = mapping)
            .AddFont("FontAwesome6FreeBrands.otf", nameof(FontAwesomeBrands), typeof(FontAwesomeBrands), mapping => FontAwesomeBrands = mapping)
            .AddFont("FontAwesome6FreeSolid.otf", nameof(FontAwesomeSolid), typeof(FontAwesomeSolid), mapping => FontAwesomeSolid = mapping)
            .AddFont("OpenSans-Semibold.ttf", nameof(OpenSansSemibold), mapping => OpenSansSemibold = mapping)
            .AddFont("OpenSans-Regular.ttf", nameof(OpenSansRegular), mapping => OpenSansRegular = mapping)
            .AddFont("MauiMaterialAssets.ttf", nameof(MauiMaterialAssets), typeof(MauiMaterialAssets), mapping => MauiMaterialAssets = mapping)
            .AddFont("Inter-Bold.ttf", nameof(InterBold), mapping => InterBold = mapping)
            .AddFont("Inter-SemiBold.ttf", nameof(InterSemiBold), mapping => InterSemiBold = mapping)
            .AddFont("Inter-Regular.ttf", nameof(InterRegular), mapping => InterRegular = mapping);
    }

    public FontMapping FontAwesomeRegular { get; private set; } = null!;

    public FontMapping MaterialIconsRegular { get; private set; } = null!;

    public FontMapping MaterialDesignIcons { get; private set; } = null!;

    public FontMapping FontAwesomeBrands { get; private set; } = null!;

    public FontMapping FontAwesomeSolid { get; private set; } = null!;

    public FontMapping OpenSansRegular { get; private set; } = null!;

    public FontMapping OpenSansSemibold { get; private set; } = null!;

    public FontMapping MauiMaterialAssets { get; private set; } = null!;

    public FontMapping InterBold { get; private set; } = null!;

    public FontMapping InterSemiBold { get; private set; } = null!;

    public FontMapping InterRegular { get; private set; } = null!;

    public ReadOnlyDictionary<string, FontMapping> Fonts
    {
        get => _fonts.AsReadOnly();
    }

    public static AppFonts Instance { get; } = new();

    public IFontsBuilder AddFont(string fontFileName, string fontFamily, Action<FontMapping>? fontMappingSetter) =>
        AddFont(fontFileName, fontFamily, null, fontMappingSetter);

    public IFontsBuilder AddFont(string fontFileName, string fontFamily, Type? fontGlyphConstantsType, Action<FontMapping>? fontMappingSetter)
    {
        var fontMapping = new FontMapping(fontFamily, fontFileName, fontGlyphConstantsType);
        _fonts.Add(fontFamily, fontMapping);
        fontMappingSetter?.Invoke(fontMapping);

        return this;
    }

    public ReadOnlyDictionary<string, FontMapping> GetFonts() => _fonts.AsReadOnly();

    public FontMapping? GetFont(string fontFamily) => _fonts.GetValueOrDefault(fontFamily);

    public bool TryGetFont(string fontFamily, [NotNullWhen(true)] out FontMapping? fontMapping) => _fonts.TryGetValue(fontFamily, out fontMapping);

    public string? GetGlyph(string fontFamily, string glyphName) => GetFont(fontFamily)?.GetGlyphValue(glyphName);
}
