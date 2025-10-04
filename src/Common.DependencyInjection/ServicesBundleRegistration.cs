using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Provides extension methods for registering services bundles with the dependency injection container.
/// </summary>
/// <remarks>
///     This static class contains extension methods that simplify the registration of <see cref="IServicesBundle" />
///     implementations with an <see cref="IServiceCollection" />. It supports both instance-based and type-based
///     registration patterns, with optional configuration support for configurable bundles.
///     <para>
///         The methods automatically handle the configuration assignment for bundles that implement
///         <see cref="ConfigurableServicesBundle" />, ensuring that configuration-dependent services
///         can access the provided configuration during registration.
///     </para>
/// </remarks>
public static class ServicesBundleRegistration
{
    /// <summary>
    ///     Registers a services bundle instance with the service collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to register the bundle with.
    /// </param>
    /// <param name="servicesBundle">
    ///     The services bundle instance to register. This bundle will have its
    ///     <see cref="IServicesBundle.Configure(IServiceCollection)" /> method called to register services.
    /// </param>
    /// <param name="configuration">
    ///     Optional configuration instance to provide to configurable bundles. If the bundle implements
    ///     <see cref="ConfigurableServicesBundle" />, this configuration will be assigned to its
    ///     <see cref="ConfigurableServicesBundle.Configuration" /> property before configuration.
    /// </param>
    /// <returns>
    ///     The same <see cref="IServiceCollection" /> instance to enable method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="servicesBundle" /> is <c>null</c>.
    /// </exception>
    /// <remarks>
    ///     This method automatically detects if the provided bundle is a <see cref="ConfigurableServicesBundle" />
    ///     and assigns the configuration before calling the bundle's configuration method. This ensures that
    ///     configuration-dependent services have access to the configuration during registration.
    /// </remarks>
    /// <example>
    ///     <code>
    /// var bundle = new MyCustomServicesBundle();
    /// services.AddServicesBundle(bundle, configuration);
    ///
    /// // For configurable bundles
    /// var configurableBundle = new DatabaseServicesBundle();
    /// services.AddServicesBundle(configurableBundle, configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddServicesBundle(this IServiceCollection services, IServicesBundle servicesBundle, IConfiguration? configuration = null)
    {
        services.NotNull(nameof(services));
        servicesBundle.NotNull(nameof(servicesBundle));

        if (servicesBundle is IConfigurationConsumer configurableServicesBundle)
        {
            configurableServicesBundle.Configuration = configuration;
        }

        servicesBundle.Configure(services);

        return services;
    }

    /// <summary>
    ///     Registers a services bundle by type with the service collection.
    /// </summary>
    /// <typeparam name="TServicesBundle">
    ///     The type of services bundle to create and register. Must implement <see cref="IServicesBundle" />
    ///     and have a parameterless constructor.
    /// </typeparam>
    /// <param name="services">
    ///     The service collection to register the bundle with.
    /// </param>
    /// <param name="configuration">
    ///     Optional configuration instance to provide to configurable bundles. If the bundle type implements
    ///     <see cref="ConfigurableServicesBundle" />, this configuration will be assigned to its
    ///     <see cref="ConfigurableServicesBundle.Configuration" /> property before configuration.
    /// </param>
    /// <returns>
    ///     The same <see cref="IServiceCollection" /> instance to enable method chaining.
    /// </returns>
    /// <remarks>
    ///     This method creates a new instance of the specified bundle type using its parameterless constructor,
    ///     then delegates to <see cref="AddServicesBundle(IServiceCollection, IServicesBundle, IConfiguration?)" />
    ///     to perform the actual registration. This provides a convenient way to register bundles without
    ///     manually instantiating them.
    ///     <para>
    ///         The bundle type must have a public parameterless constructor. If the bundle implements
    ///         <see cref="ConfigurableServicesBundle" />, the provided configuration will be automatically
    ///         assigned before the bundle's configuration method is called.
    ///     </para>
    /// </remarks>
    /// <exception cref="MissingMethodException">
    ///     Thrown when <typeparamref name="TServicesBundle" /> does not have a public parameterless constructor.
    /// </exception>
    /// <example>
    ///     <code>
    /// // Register a simple bundle
    /// services.AddServicesBundle&lt;LoggingServicesBundle&gt;();
    ///
    /// // Register a configurable bundle with configuration
    /// services.AddServicesBundle&lt;DatabaseServicesBundle&gt;(configuration);
    ///
    /// // Method chaining
    /// services.AddServicesBundle&lt;CacheServicesBundle&gt;(configuration)
    ///         .AddServicesBundle&lt;SecurityServicesBundle&gt;(configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddServicesBundle<TServicesBundle>(this IServiceCollection services, IConfiguration? configuration = null)
        where TServicesBundle : IServicesBundle, new()
    {
        var servicesBundle = new TServicesBundle();

        AddServicesBundle(services, servicesBundle, configuration);

        return services;
    }
}
