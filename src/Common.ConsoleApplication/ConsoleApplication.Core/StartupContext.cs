using System.Collections.Generic;

namespace Ploch.Common.ConsoleApplication.Core
{
    public class StartupContext
    {
        public IEnumerable<string> Arguments { get; }

        public StartupContext(IEnumerable<string> arguments)
        {
            Arguments = arguments;
        }
    }
}