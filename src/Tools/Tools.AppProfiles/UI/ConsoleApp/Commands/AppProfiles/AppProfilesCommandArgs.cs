using CommandLine;

namespace Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.AppProfiles
{
    public enum AppProfilesAction
    {
        Save,
        Load
    }
    
    public class AppProfilesCommandArgs
    {
        [Option('a', "action", Default = AppProfilesAction.Save)]
        public AppProfilesAction Action { get; set; }

        [Option('f', "filename")]
        public string FileName { get; set; }
        
        
    }
}