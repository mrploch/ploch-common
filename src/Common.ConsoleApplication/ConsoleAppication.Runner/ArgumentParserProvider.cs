using CommandLine;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class ArgumentParserProvider
    {
        public Parser GetParser()
        {
            var parser = new Parser(settings =>
            {
                settings.HelpWriter = null;
            });

            return parser;
        }
    }
}