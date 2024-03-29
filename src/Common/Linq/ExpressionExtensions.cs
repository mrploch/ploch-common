﻿using System;
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
    ///     Gets the member name from an expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>Member name.</returns>
    /// <exception cref="InvalidOperationException">Not a member expression.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="expression" /> value is <c>null</c>.</exception>
    public static string GetMemberName(this Expression<Action> expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Body switch
               {
                   MemberExpression memberExpressionBody => memberExpressionBody.Member.Name,
                   MethodCallExpression methodCallExpression => methodCallExpression.Method.Name,
                   _ => throw new InvalidOperationException("Not a member expression!")
               };
    }

    /// <summary>
    ///     Gets the member name from an expression.
    /// </summary>
    /// <typeparam name="TMember">The member.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>Member name.</returns>
    /// <exception cref="InvalidOperationException">Not a member expression.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="expression" /> value is <c>null</c>.</exception>
    public static string GetMemberName<TMember>(this Expression<Func<TMember>> expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Body switch
               {
                   MemberExpression memberExpressionBody => memberExpressionBody.Member.Name,
                   MethodCallExpression methodCallExpression => methodCallExpression.Method.Name,
                   _ => throw new InvalidOperationException("Not a member expression!")
               };
    }

    /// <summary>
    ///     Gets the member name from an expression.
    /// </summary>
    /// <typeparam name="TType">Member parent type.</typeparam>
    /// <typeparam name="TMember">Member type.</typeparam>
    /// <param name="expression">Expression.</param>
    /// <returns>Member name.</returns>
    /// <exception cref="InvalidOperationException">Not a member expression and not unary expression for member.</exception>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="expression" /> value is <c>null</c> and the argument is not modified
    ///     since it is initialized.
    /// </exception>
    public static string GetMemberName<TType, TMember>(this Expression<Func<TType, TMember>> expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Body switch
               {
                   MemberExpression memberExpressionBody => memberExpressionBody.Member.Name,
                   MethodCallExpression methodCallExpression => methodCallExpression.Method.Name,

                   // Might be an implicit cast
                   UnaryExpression { Operand: MemberExpression memberExpression } => memberExpression.Member.Name,
                   _ => throw new InvalidOperationException("Not a member expression and not unary expression for member.")
               };
    }

    /// <summary>
    ///     Retrieves property information based on the provided property selector expression.
    /// </summary>
    /// <typeparam name="TType">The type of the object.</typeparam>
    /// <typeparam name="TMember">The type of the property.</typeparam>
    /// <param name="obj">The object instance.</param>
    /// <param name="propertySelector">The property selector expression.</param>
    /// <returns>An instance of <see cref="IOwnedPropertyInfo{TType, TMember}" /> representing the property information.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the provided <paramref name="propertySelector" /> is not a
    ///     property expression.
    /// </exception>
    public static IOwnedPropertyInfo<TType, TMember> GetProperty<TType, TMember>(this TType obj, Expression<Func<TType, TMember>> propertySelector)
    {
        Guard.Argument(propertySelector, nameof(propertySelector)).NotNull();

        if (propertySelector.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException($"Provided {propertySelector} is not a property expression.");
        }

        var propertyInfo = memberExpression.Member as PropertyInfo ?? throw new InvalidOperationException($"Provided {propertySelector} is not a property expression.");

        return new OwnedPropertyInfo<TType, TMember>(propertyInfo, obj);
    }
}