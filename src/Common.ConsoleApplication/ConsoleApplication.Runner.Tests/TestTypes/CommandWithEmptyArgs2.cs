using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class CommandWithEmptyArgs2 : ICommand<EmptyArgs2>
    {
        /// <inheritdoc />
        public void Execute(EmptyArgs2 options)
        { }
    }
}