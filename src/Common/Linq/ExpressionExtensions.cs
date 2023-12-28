using System;
using System.Linq.Expressions;
using System.Reflection;
using Dawn;

namespace Ploch.Common.Linq;

/// <summary>
///     Extension methods for <see cref="Expression" />.
/// </summary>
/// <remarks>
///     Contains various utility extension methods for working with <see cref="Expression" /> objects.
/// </remarks>
/// <seealso cref="Expression" />
public static class ExpressionExtensions
{
    /// <summary>
    ///     Gets the member name from an expression
    /// </summary>
    /// <typeparam name="TType">The type.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>Member name</returns>
    /// <exception cref="InvalidOperationException">Not a member expression.</exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="expression" /> value is <c>null</c>.</exception>
    public static string GetMemberName<TType>(this Expression<Action<TType>> expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        if (expression.Body is MemberExpression memberExpressionBody)
        {
            return memberExpressionBody.Member.Name;
        }

        if (expression.Body is MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Method.Name;
        }

        throw new InvalidOperationException("Not a member expression!");
    }

    /// <summary>
    ///     Gets the member name from an expression.
    /// </summary>
    /// <typeparam name="TMember">The member.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>Member name</returns>
    /// <exception cref="InvalidOperationException">Not a member expression.</exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="expression" /> value is <c>null</c>.</exception>
    public static string GetMemberName<TMember>(this Expression<Func<TMember>> expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        if (expression.Body is MemberExpression memberExpressionBody)
        {
            return memberExpressionBody.Member.Name;
        }

        if (expression.Body is MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Method.Name;
        }

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
    /// <exception cref="T:System.ArgumentNullException">
    ///     <paramref name="argument" /> value is <c>null</c> and the argument is not modified
    ///     since it is initialized.
    /// </exception>
    public static string GetMemberName<TType, TMember>(this Expression<Func<TType, TMember>> expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        if (expression.Body is MemberExpression memberExpressionBody)
        {
            return memberExpressionBody.Member.Name;
        }

        if (expression.Body is MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Method.Name;
        }

        // Might be an implicit cast
        if (expression.Body is UnaryExpression { Operand: MemberExpression memberExpression })
        {
            return memberExpression.Member.Name;
        }

        throw new InvalidOperationException("Not a member expression and not unary expression for member.");
    }

    public static IOwnedPropertyInfo<TType, TMember> GetProperty<TType, TMember>(this TType obj, Expression<Func<TType, TMember>> propertySelector)
    {
        Guard.Argument(propertySelector, nameof(propertySelector)).NotNull();

        if (propertySelector.Body is MemberExpression memberExpression)
        {
            var propertyInfo = memberExpression.Member as PropertyInfo ??
                               throw new InvalidOperationException($"Provided {propertySelector} is not a property expression.");

            return new OwnedPropertyInfo<TType, TMember>(propertyInfo, obj);
        }

        throw new InvalidOperationException($"Provided {propertySelector} is not a property expression.");
    }
}