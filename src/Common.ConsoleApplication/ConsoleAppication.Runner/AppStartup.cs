using System;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class AppStartup
    {
        private static readonly AppBootstrapper _bootstrapper = new AppBootstrapper();

        private static readonly AppStartup _instance = new AppStartup();

        public static void ExecuteApp(string[] args, params Type[] commands)
        {
            _bootstrapper.ExecuteApp(args, commands);
        }

        public static void ExecuteApp<TApp>(string[] args) where TApp : ICommand
        {
            _bootstrapper.ExecuteApp<TApp>(args);
        }

        public static void ExecuteApp<TApp, TArgs>(string[] args) where TApp: ICommand<TArgs>
        {
            _bootstrapper.ExecuteApp<TApp, TArgs>(args);
        }
    }
}