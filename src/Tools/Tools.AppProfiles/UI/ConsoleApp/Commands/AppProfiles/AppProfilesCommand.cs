using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.AppProfiles
{
    public class AppProfilesCommand : ICommand<AppProfilesCommandArgs>
    {
        private readonly IOutput _output;

        public AppProfilesCommand(IOutput output)
        {
            _output = output;
        }

        /// <inheritdoc />
        public void Execute(AppProfilesCommandArgs options)
        {
            _output.WriteLine($"App Profiles - action: {options.Action}");
            
        }

       
    }
}