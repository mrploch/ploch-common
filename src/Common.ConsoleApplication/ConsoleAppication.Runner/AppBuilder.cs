using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ConsoleApplication.Runner;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Core
{
    public class AppBuilder
    {
        private readonly ICollection<IServicesBundle> _appServices;

        private readonly DelegatingServicesBundle _servicesBundle = new DelegatingServicesBundle();

        public AppBuilder()
        {
            _appServices = new List<IServicesBundle>() { new DefaultServices(), _servicesBundle};
        }

        public AppBuilder WithServices(IServicesBundle appServices)
        {
            _appServices.Add(appServices);
            return this;
        }

        public AppBuilder AddEvents(IAppEvents events)
        {
            _servicesBundle.Configure(f => f.AddSingleton(events));
            return this;
        }
        
        public AppBootstrapper Bootstrapper()
        {
            return new AppBootstrapper(new CompositeServicesBundle(_appServices.ToArray()));
        }
    }
}