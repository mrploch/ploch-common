using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class CommandWithEmptyArgs2 : AppCommand<EmptyArgs2>
    {
        /// <inheritdoc />
        public override void Execute(EmptyArgs2 options)
        { }
    }
}