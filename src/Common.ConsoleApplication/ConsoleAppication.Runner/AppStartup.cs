using System;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public static class AppStartup
    {
        public static IAppBootstrapper Default()
        {
            return Configure().WithServiceProvider(services => services.BuildServiceProvider()).Bootstrapper();
        }

        public static AppBuilder Configure()
        {
            return new();
        }

    }
}