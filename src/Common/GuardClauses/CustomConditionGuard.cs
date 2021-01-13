using System;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable CheckNamespace

namespace Ardalis.GuardClauses
{
    public static class CustomConditionGuard
    {
        public static void ArgCondition(this IGuardClause guardClause, Expression<Func<bool>> condition, string parameterName)
        {
            ArgCondition(guardClause, condition.Compile()(), parameterName, condition.ToString());
        }

        public static void ArgCondition(this IGuardClause guardClause, bool condition, string parameterName, string message)
        {
            if (condition)
            {
                throw new ArgumentException($"Validation of {parameterName} argument failed: {message}", parameterName);
            }
        }

        public static void OperationCondition(this IGuardClause guardClause, Expression<Func<bool>> condition)
        {
            OperationCondition(guardClause, condition.Compile()(), $"Validation of operation condition failed: {condition}");
        }

        public static void OperationCondition(this IGuardClause guardClause, bool condition, string message)
        {
            
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}