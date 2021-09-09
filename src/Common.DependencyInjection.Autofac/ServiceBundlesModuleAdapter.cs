using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection.Autofac
{
    public class ServiceBundlesModuleAdapter : Module
    {
        private readonly IEnumerable<IServicesBundle> _bundle;
        private readonly IServiceCollection _serviceCollection;

        public ServiceBundlesModuleAdapter(params IServicesBundle[] bundles) : this(new ServiceCollection(), bundles)
        { }

        public ServiceBundlesModuleAdapter(IServiceCollection serviceCollection, params IServicesBundle[] bundles) : this(serviceCollection,
                                                                                                                          (IEnumerable<IServicesBundle>)bundles)
        { }

        public ServiceBundlesModuleAdapter(IEnumerable<IServicesBundle> bundle) : this(new ServiceCollection(), bundle)
        { }

        public ServiceBundlesModuleAdapter(IServiceCollection serviceCollection, IEnumerable<IServicesBundle> bundle)
        {
            _serviceCollection = serviceCollection;
            _bundle = bundle;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var serviceCollection = new ServiceCollection();
            foreach (var servicesBundle in _bundle)
            {
                servicesBundle.Configure(serviceCollection);
            }
            builder.Populate(serviceCollection);
        }
    }
}