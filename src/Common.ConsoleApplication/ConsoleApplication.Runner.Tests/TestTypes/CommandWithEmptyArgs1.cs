using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class CommandWithEmptyArgs1 : ICommand<EmptyArgs1>
    {
        /// <inheritdoc />
        public void Execute(EmptyArgs1 options)
        { }
    }
}