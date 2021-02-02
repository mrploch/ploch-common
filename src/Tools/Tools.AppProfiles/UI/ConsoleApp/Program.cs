using Ploch.Common.ConsoleApplication.Runner;
using Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.AppProfiles;

namespace Ploch.Tools.AppProfiles.UI.ConsoleApp
{
    static class Program
    {
        public static void Main(string[] args) => AppStartup.ExecuteApp<AppProfilesCommand, AppProfilesCommandArgs>(args);
    }
}
