using System;
using Ploch.Common.ConsoleApplication.Runner;

namespace Ploch.Common.ConsoleApplication.Core
{
    public class AppBuilder
    {
        private IAppServices _appServices = new DefaultServices();

        public AppBuilder WithServices(IAppServices appServices)
        {
            _appServices = appServices;
            return this;
        }
        
        public AppBootstrapper Bootstrapper()
        {
            var bootstrapper = new AppBootstrapper(_appServices ?? new DefaultServices());
            return bootstrapper;
        }
    }
}