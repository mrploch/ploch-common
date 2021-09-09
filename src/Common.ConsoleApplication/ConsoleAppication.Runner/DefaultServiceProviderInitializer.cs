using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class DefaultServiceProviderInitializer : ServiceProviderInitializer
    {
        public DefaultServiceProviderInitializer([CanBeNull] IServicesBundle? appServices = null)
            : base(appServices, serviceCollection => serviceCollection.BuildServiceProvider())
        {
        }
    }
}