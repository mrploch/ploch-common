using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class CommandWithEmptyArgs1 : AppCommand<EmptyArgs1>
    {
        /// <inheritdoc />
        public override void Execute(EmptyArgs1 options)
        { }
    }
}