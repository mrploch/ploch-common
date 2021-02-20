// unset

using Dawn;
using JetBrains.Annotations;
using System;
using System.Diagnostics;

namespace Ardalis.GuardClauses
{
    public static class CustomGuards
    {
        /// <summary>Requires the nullable argument not to be <c>null</c>.</summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns>A new <see cref="T:Dawn.Guard.ArgumentInfo`1" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="argument" /> value is <c>null</c> and the argument is not modified
        ///     since it is initialized.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="argument" /> value is <c>null</c> and the argument is modified after
        ///     its initialization.
        /// </exception>
        [AssertionMethod]
        [DebuggerStepThrough]
        public static Dawn.Guard.ArgumentInfo<T> NotNullIfClass<T>(
            in this Dawn.Guard.ArgumentInfo<T> argument,
            string? message = null)
        {
            if (!argument.HasValue())
            {
                var msg = message ?? $"Argument {argument.Name} was null!";
                {
                    throw Dawn.Guard.Fail(!argument.Modified
                                              ? (Exception)new ArgumentNullException(argument.Name, msg)
                                              : (Exception)new ArgumentException(msg, argument.Name));
                }
            }
            return new Dawn.Guard.ArgumentInfo<T>(argument.Value, argument.Name, argument.Modified, argument.Secure);
        }
    }
}