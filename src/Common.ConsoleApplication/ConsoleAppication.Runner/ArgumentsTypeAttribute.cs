using System;
using Dawn;
using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    /// <summary>
    ///     Specifies which arguments type to use when parsing command line args for this <see cref="ICommand{TOptions}" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ArgumentsTypeAttribute : Attribute
    {
        /// <inheritdoc />
        public ArgumentsTypeAttribute([NotNull] Type argumentsType)
        {
            ArgumentsType = argumentsType;
        }

        [NotNull] public Type ArgumentsType { get; }
    }
}