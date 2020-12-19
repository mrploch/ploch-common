using System;
using System.Collections.Generic;
using CommandLine;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class AdvancedPropertiesArgs1
    {
        [Option('d')] public DateTime DateProperty1 { get; set; }

        [Option('l')] public IList<string> ListProperty1 { get; set; }
    }
}