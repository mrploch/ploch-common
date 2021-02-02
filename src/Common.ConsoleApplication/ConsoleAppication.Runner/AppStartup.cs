using System;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class AppStartup
    {
        public static void ExecuteApp(string[] args, params Type[] commands)
        {
            ExecuteApp(args, new AppBootstrapper(), commands);
        }

        public static void ExecuteApp(string[] args, AppBootstrapper bootstrapper, params Type[] commands)
        {
            bootstrapper.ExecuteApp(args, commands);
        }

        public static void ExecuteApp<TApp>(string[] args) where TApp : ICommand
        {
            ExecuteApp<TApp>(args, new AppBootstrapper());
        }

        public static void ExecuteApp<TApp>(string[] args, AppBootstrapper bootstrapper) where TApp : ICommand
        {
            bootstrapper.ExecuteApp<TApp>(args);
        }

        public static void ExecuteApp<TApp, TArgs>(string[] args, AppBootstrapper? bootstrapper = null) where TApp: ICommand<TArgs>
        {
            bootstrapper ??= new AppBootstrapper();
            bootstrapper.ExecuteApp<TApp, TArgs>(args);
        }
    }
}