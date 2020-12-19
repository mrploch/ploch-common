using System;
using System.Linq.Expressions;

namespace Ploch.Common
{
    public static class ExpressionExtensions
    {
        /// <summary>
        ///     Gets the member name from an expression
        /// </summary>
        /// <typeparam name="TMember">Member</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Member name</returns>
        /// <exception cref="InvalidOperationException">Not a member expression!</exception>
        public static string GetMemberName<TMember>(this Expression<Func<TMember>> expression)
        {
            if (expression.Body is MemberExpression memberExpressionBody) return memberExpressionBody.Member.Name;

            if (expression.Body is MethodCallExpression methodCallExpression) return methodCallExpression.Method.Name;

            throw new InvalidOperationException("Not a member expression!");
        }

        /// <summary>
        ///     Gets the member name from an expression
        /// </summary>
        /// <typeparam name="TType">Member parent type</typeparam>
        /// <typeparam name="TMember">Member type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Member name</returns>
        /// <exception cref="InvalidOperationException">Not a member expression and not unary expression for member.</exception>
        public static string GetMemberName<TType, TMember>(this Expression<Func<TType, TMember>> expression)
        {
            if (!(expression.Body is MemberExpression memberExpressionBody))
            {
                // Might be an implicit cast
                if (!(expression.Body is UnaryExpression unaryExpression))
                    throw new InvalidOperationException("Not a member expression and not unary expression for member.");
                memberExpressionBody = unaryExpression.Operand as MemberExpression;
                if (memberExpressionBody == null) throw new InvalidOperationException("Not a member expression.");
            }

            return memberExpressionBody.Member.Name;
        }
    }
}