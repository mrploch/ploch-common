using Microsoft.Extensions.Configuration;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Represents a consumer for configuration settings.
/// </summary>
/// <remarks>
///     This interface allows for the injection and utilization of a <see cref="IConfiguration" /> instance.
///     <para>
///         It enables setting a configuration object that can be used within the implementing classes
///         for accessing application settings from various configuration sources such as JSON,
///         environment variables, or command-line arguments.
///     </para>
/// </remarks>
public interface IConfigurationConsumer
{
    /// <summary>
    ///     Provides access to an <see cref="IConfiguration" /> instance used to manage application settings and configuration data.
    /// </summary>
    /// <remarks>
    ///     This property returns an <see cref="IConfiguration" /> object, which can be used to retrieve configuration
    ///     information sourced from various providers like JSON files, environment variables, or other custom sources.
    ///     Primarily utilized within dependency injection scenarios, this property facilitates centralized handling
    ///     and retrieval of configurations for services or components within an application.
    /// </remarks>
    IConfiguration? Configuration { set; }
}
