using Microsoft.Extensions.Configuration;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Represents a provider responsible for accessing and supplying configuration settings.
/// </summary>
/// <remarks>
///     This interface exposes a property to retrieve the current configuration instance.
///     It is typically implemented to provide a centralized access point for application configuration values.
///     The configuration may include settings from various sources, such as JSON files, environment variables, or user secrets.
/// </remarks>
public interface IOptionalConfigurationProvider
{
    /// <summary>
    ///     Gets the current configuration instance accessible within the implementation of the interface.
    /// </summary>
    /// <remarks>
    ///     This property provides access to an <see cref="IConfiguration" /> instance that contains the application's
    ///     configured settings. Typically, the configuration data is sourced from a combination of providers like JSON files,
    ///     environment variables, command-line arguments, or user secrets.
    ///     In implementations of interfaces such as <c>IServicesBundle</c>, it can be used to supply necessary configuration
    ///     data to registered service dependencies. This property enables flexibility and centralized management of
    ///     configuration for applications.
    /// </remarks>
    IConfiguration? Configuration { get; }
}
