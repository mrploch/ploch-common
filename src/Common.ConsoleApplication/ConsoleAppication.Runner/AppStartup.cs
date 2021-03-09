using System;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public static class AppStartup
    {
        public static IAppBootstrapper Default()
        {
            return Configure().Bootstrapper();
        }

        public static AppBuilder Configure()
        {
            return new AppBuilder();
        }

    
    }
}