using System;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable CheckNamespace

namespace Ardalis.GuardClauses
{
    /// <summary>
    /// Class CustomConditionGuard.
    /// </summary>
    public static class CustomConditionGuard
    {
        /// <summary>
        /// Argument condition.
        /// </summary>
        /// <param name="guardClause">The guard clause.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void ArgCondition(this IGuardClause guardClause, Expression<Func<bool>> condition, string parameterName)
        {
            ArgCondition(guardClause, condition.Compile()(), parameterName, condition.ToString());
        }

        /// <summary>
        /// Argument condition.
        /// </summary>
        /// <param name="guardClause">The guard clause.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentException">Validation of {parameterName} argument failed: {message}</exception>
        public static void ArgCondition(this IGuardClause guardClause, bool condition, string parameterName, string message)
        {
            if (condition)
            {
                throw new ArgumentException($"Validation of {parameterName} argument failed: {message}", parameterName);
            }
        }

        /// <summary>
        /// Operation condition.
        /// </summary>
        /// <param name="guardClause">The guard clause.</param>
        /// <param name="condition">The condition.</param>
        public static void OperationCondition(this IGuardClause guardClause, Expression<Func<bool>> condition)
        {
            OperationCondition(guardClause, condition.Compile()(), $"Validation of operation condition failed: {condition}");
        }

        /// <summary>
        /// Operation condition.
        /// </summary>
        /// <param name="guardClause">The guard clause.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void OperationCondition(this IGuardClause guardClause, bool condition, string message)
        {
            
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}