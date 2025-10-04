using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Extensions.Configuration;
using Ploch.Common.Linq;

namespace Common.Extensions.Configuration;

public static class ConfigurationOptionsExtensions
{
    public static IServiceCollection AddConfigurationSection<TSection>(this IServiceCollection services, IConfiguration configuration)
        where TSection : class, new() => services.Configure<TSection>(configuration.GetSection(GetSectionName<TSection>()));

    public static IServiceCollection AddConfigurationOptions<TMainSection, TSubSection>(this IServiceCollection services,
                                                                                        IConfiguration configuration,
                                                                                        Expression<Func<TMainSection, TSubSection>> subSectionProperty)
        where TSubSection : class
    {
        var configurationSection = configuration.GetSection(GetSectionName<TMainSection>()).GetSection(subSectionProperty.GetMemberName());

        return services.Configure<TSubSection>(configurationSection);
    }

    private static string GetSectionName<TSection>()
    {
        var secionName = typeof(TSection).GetCustomAttribute<ConfigurationSectionAttribute>()?.SectionName ?? typeof(TSection).Name;

        return secionName;
    }
}
