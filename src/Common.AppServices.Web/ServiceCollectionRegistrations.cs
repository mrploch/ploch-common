using Ploch.Common.AppServices.Security;

namespace Ploch.Common.AppServices.Web;

/// <summary>
///     Provides a set of extension methods for registering the library services in the
///     dependency injection container.
/// </summary>
public static class ServiceCollectionRegistrations
{
    /// <summary>
    ///     Adds the user information provider service to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the user information provider will be added.</param>
    /// <returns>The same service collection instance, with the user information provider registered.</returns>
    public static IServiceCollection AddUserInfoProvider(this IServiceCollection services)
    {
        services.AddSingleton<IUserInfoProvider, HttpContextUserInfoProvider>();

        return services;
    }
}
