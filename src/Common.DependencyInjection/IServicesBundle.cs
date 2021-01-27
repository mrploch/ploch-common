using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection
{
    /// <summary>
    /// Represents a collection of services that should be added to <see cref="IServiceCollection"/>
    /// </summary>
    public interface IServicesBundle
    {
        /// <summary>
        /// Configures a <c>IServiceCollection</c> instance.
        /// </summary>
        /// <remarks>
        /// Implementations of this method should add all required services for this particular Services Bundle.
        /// Method will be executed when a service provider is built.
        /// </remarks>
        /// <param name="serviceCollection">A service collection.</param>
        void Configure(IServiceCollection serviceCollection);
    }
}