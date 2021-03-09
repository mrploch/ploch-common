using System.Collections.Generic;
using CommandLine;

namespace Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.InstalledSoftware
{
    [Verb("installed-software")]
    public class GetInstalledSoftwareArgs
    {
        public string? OutputPath { get; set; }

        public string? NameFilter { get; set; }

        public IEnumerable<string>? PropertyFilter { get; set; }
        
        
    }
}