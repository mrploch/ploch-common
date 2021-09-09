using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ploch.Common.ConsoleApplication.Core;
using Ploch.Common.ConsoleApplication.Runner.Configuration;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class AppBuilder
    {
        private readonly ICollection<IServicesBundle> _appServices;

        private readonly IDictionary<string, Action<AppBuilder>> _defaultActions;

        private readonly DelegatingServicesBundle _servicesBundle = new();

        private Func<IServiceCollection, IServiceProvider>? _serviceProviderFunc;

        public AppBuilder()
        {
            _appServices = new List<IServicesBundle> {_servicesBundle};
            _serviceProviderFunc = serviceCollection => serviceCollection.BuildServiceProvider();
            _defaultActions = new Dictionary<string, Action<AppBuilder>>
                              {
                                  {nameof(WithDefaultConfiguration), builder => builder.WithDefaultConfiguration(out _) }
                              };
        }

        public AppBuilder WithServices(params IServicesBundle[] servicesBundles)
        {
            foreach (var appServices in servicesBundles)
            {
                _appServices.Add(appServices);
            }

            return this;
        }

        public AppBuilder WithServiceProvider([NotNull] Func<IServiceCollection, IServiceProvider> serviceProviderFunc)
        {
            _serviceProviderFunc = Guard.Argument(serviceProviderFunc, nameof(serviceProviderFunc)).NotNull();
            return this;
        }


        public AppBuilder WithLogging(Action<ILoggingBuilder>? configurationActions = null)
        {
            _servicesBundle.Configure(serviceCollection => serviceCollection.AddLogging(builder =>
                                                                                        {
                                                                                            builder.AddConsole();
                                                                                            configurationActions?.Invoke(builder);
                                                                                        }));
            _defaultActions.Remove(nameof(WithLogging));

            return this;
        }

        public AppBuilder WithDefaultConfiguration(out IConfiguration configuration,
                                                   IConfigurationBuilder? configurationBuilder = null,
                                                   Action<IConfigurationBuilder>? configurationAction = null)
        {
            configurationBuilder ??= new ConfigurationBuilder();
            configurationBuilder.UseDefaultConfiguration();
            configurationAction?.Invoke(configurationBuilder);
            configuration = configurationBuilder.Build();

            return WithConfiguration(configuration);
        }

        public AppBuilder WithConfiguration(IConfiguration configuration)
        {
            _servicesBundle.Configure(services => services.AddSingleton(configuration));
            _defaultActions.Remove(nameof(WithDefaultConfiguration));

            return this;
        }

        public AppBuilder AddEvents(IAppEvents events)
        {
            _servicesBundle.Configure(f => f.AddSingleton(events));
            return this;
        }

        public AppBootstrapper Bootstrapper()
        {
            foreach (var action in _defaultActions.Values)
            {
                action(this);
            }

            return new AppBootstrapper(new ServiceProviderInitializer(new CompositeServicesBundle(_appServices.ToArray()), _serviceProviderFunc));
        }
    }
}