using System;
using System.Collections.Generic;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public interface IAppBootstrapper
    {
        /// <summary>
        ///     Executes the app.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <exception cref="InvalidOperationException">Thrown when command of type <typeparamref name="TApp"/> could not be resolved.</exception>
        void ExecuteApp<TApp>(string[] args) where TApp : ICommand;

        /// <summary>
        ///     Executes the apps.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="commands">The commands.</param>
        /// <exception cref="ArgumentException">Thrown when provided commands do not implement <see cref="ICommand{TOptions}" interface./></exception>
        void ExecuteApp(IEnumerable<string> args, IEnumerable<Type> commands);

        /// <summary>
        ///     Executes the app.
        /// </summary>
        /// <param name="args">The args.</param>
        void ExecuteApp<TApp, TArgs>(string[] args) where TApp : ICommand<TArgs>;
    }
}