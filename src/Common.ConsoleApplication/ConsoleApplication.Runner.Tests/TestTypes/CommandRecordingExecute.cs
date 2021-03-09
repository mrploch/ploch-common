using System.Threading;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class CommandRecordingExecute<TArgs> : ICommand<TArgs>
    {
        private static int _executeCallCount;
        public static TArgs Args { get; private set; }

        public static int ExecuteCallCount => _executeCallCount;

        public CommandRecordingExecute()
        {
            _executeCallCount = 0;
        }

        /// <inheritdoc />
        public void Execute(TArgs options)
        {
            Interlocked.Increment(ref _executeCallCount);
            Args = options;
        }
    }
}