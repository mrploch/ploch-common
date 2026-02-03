using System.Reflection;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Maui.Configuration;
public static class JsonConfigurationResourceFileRegistrations
{
    public static TConfigurationBuilder AddResourceJsonFileConfiguration<TConfigurationBuilder>(this TConfigurationBuilder builder)
        where TConfigurationBuilder : IConfigurationBuilder
    {
        var platform = DeviceInfo.Current.Platform.ToString();
        builder.AddJsonStream(GetConfigurationFileStream(null))
               .AddJsonStream(GetConfigurationFileStream(platform));

        return builder;
    }

    public static MauiAppBuilder AddOptions<TOptions>(this MauiAppBuilder builder, string? sectionName = null, bool optional = false)
        where TOptions : class
    {
        sectionName ??= typeof(TOptions).Name;
        builder.Services.Configure<TOptions>(optional ? builder.Configuration.GetSection(sectionName) : builder.Configuration.GetRequiredSection(sectionName));

        return builder;
    }

    private static Stream GetConfigurationFileStream(string? platform)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            var resourceName = platform == null ? $"{assemblyName}.appsettings.json" : $"{assemblyName}.appsettings.{platform}.json";
            var resourceStream = assembly.GetManifestResourceStream(resourceName);

            if (resourceStream == null)
            {
                throw new InvalidOperationException($"Resource {resourceName} not found in assembly {assembly.FullName}");
            }

            return resourceStream;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw;
        }
    }
}
