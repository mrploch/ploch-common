using System;

namespace Ploch.Common.ConsoleApplication.Core
{
    /// <summary>
    ///     Interface for extensions that are called during specific application events.
    /// </summary>
    public interface IAppEvents
    {
        /// <summary>
        ///     Controls the order of <see cref="IAppEvents" /> execution.
        /// </summary>
        /// <remarks>
        ///     Resolved implementations will be executed in ascending order. Implementations with
        ///     lower numbers will be executed before higher numbers.
        ///     By default, the value is -1 (<see cref="Unordered" />) which means that order is not important.
        /// </remarks>
        int Order { get; }

        void OnStartup(IServiceProvider serviceProvider);

        void OnShutdown(IServiceProvider serviceProvider);
    }
}