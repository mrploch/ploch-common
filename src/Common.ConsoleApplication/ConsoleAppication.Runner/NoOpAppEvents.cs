using System;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    /// <summary>
    /// Implementation of <see cref="IAppEvents"/> that does nothing.
    /// </summary>
    public class NoOpAppEvents : IAppEvents
    {
        /// <inheritdoc />
        public void OnStartup(IServiceProvider serviceProvider)
        {
            // NO-OP
        }

        /// <inheritdoc />
        public void OnShutdown(IServiceProvider serviceProvider)
        {
            // NO-OP
        }
    }
}