using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Core;
using Ploch.Common.ConsoleApplication.Runner;
using Ploch.Common.DependencyInjection;
using Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.AppProfiles;

namespace Ploch.Tools.AppProfiles.UI.ConsoleApp
{
    static class Program
    {
        public static void Main(string[] args)
        {
            AppStartup.Default().ExecuteApp<AppProfilesCommand, AppProfilesCommandArgs>(args);
        }
    }

}