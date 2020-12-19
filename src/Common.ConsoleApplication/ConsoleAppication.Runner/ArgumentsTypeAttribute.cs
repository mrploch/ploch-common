using System;
using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Core;
using Validation;

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
            Requires.NotNull(argumentsType, nameof(argumentsType));
            ArgumentsType = argumentsType;
        }

        [NotNull] public Type ArgumentsType { get; }
    }
}