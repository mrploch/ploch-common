using System;
using System.Collections.Generic;
using Dawn;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class ServiceProviderInitializer
    {
        private readonly IServicesBundle? _appServices;
        private readonly Func<IServiceCollection, IServiceProvider> _serviceProviderFunc;

        public ServiceProviderInitializer(IServicesBundle? appServices, [NotNull] Func<IServiceCollection, IServiceProvider> serviceProviderFunc)
        {
            _appServices = appServices;
            _serviceProviderFunc = Guard.Argument(serviceProviderFunc, nameof(serviceProviderFunc)).NotNull();
        }

        /// <summary>
        ///     Initializes the service provider.
        /// </summary>
        /// <returns>An IServiceProvider.</returns>
        public IServiceProvider CreateServiceProvider(IEnumerable<string> args, object parsedArgs, params Type[] commandTypes)
        {
            var services = new ServiceCollection();
            services.AddSingleton(parsedArgs.GetType(), parsedArgs);
            var requiredServices = new RequiredServices(args, commandTypes);
            requiredServices.Configure(services);
            _appServices?.Configure(services);
            var provider = _serviceProviderFunc(services);
            return provider ?? throw new InvalidOperationException("Could not initialize Service Provider!");
        }
    }
}