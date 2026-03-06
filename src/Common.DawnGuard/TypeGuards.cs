using System;
using Dawn;

namespace Ploch.Common.DawnGuard;

/// <summary>
///     Provides utility methods for type guards.
/// </summary>
/// <remarks>
/// This class is deprecated and no longer maintained. It'll be removed in new versions of the library.
/// It is recommended to use the new helpers for argument validation available in the <c>Ploch.Common</c> package, specifically in the <c>Ploch.Common.ArgumentChecking</c> namespace.
/// <seealso cref="Ploch.Common.ArgumentChecking.Guard"/>
/// <seealso cref="Ploch.Common.ArgumentChecking.PathGuard"/>
/// </remarks>
[Obsolete($"The DawnGuard library is deprecated and no longer maintained. " +
          $"New helpers for argument validation are available in the {nameof(Ploch)}.{nameof(Common)} package, {nameof(ArgumentChecking)} namespace. " +
          $"For example the {nameof(ArgumentChecking.Guard)}.")]
public static class TypeGuards
{
    /// <summary>
    ///     Checks if the provided type is assignable to the specified type.
    /// </summary>
    /// <param name="argument">The type to check.</param>
    /// <param name="type">The type to which the argument should be assignable.</param>
    /// <returns>The original argument info for method chaining.</returns>
    /// <exception cref="ArgumentNullException">If the argument is null.</exception>
    public static ref readonly Guard.ArgumentInfo<Type> AssignableTo(in this Guard.ArgumentInfo<Type> argument, Type type)
    {
        Guard.Argument(type, nameof(type)).NotNull();

        // Check whether the GUID is empty.
        if (argument.Value == null)
        {
            throw Guard.Fail(new ArgumentNullException(argument.Name, $"Argument {argument.Name} is null."));
        }

        return ref AssignableToOrNull(argument, type);
    }

    /// <summary>
    ///     Checks if the provided type is assignable to the type of <typeparamref name="TExpected" />.
    /// </summary>
    /// <param name="argument">The type to check.</param>
    /// <typeparam name="TExpected">The type to check against.</typeparam>
    /// <returns>The original argument info for method chaining.</returns>
    public static ref readonly Guard.ArgumentInfo<Type> AssignableTo<TExpected>(in this Guard.ArgumentInfo<Type> argument) =>
        ref AssignableTo(argument, typeof(TExpected));

    /// <summary>
    ///     Checks if the value of the argument is assignable to the specified type.
    /// </summary>
    /// <param name="argument">The reference to the argument object.</param>
    /// <param name="type">The type to check against.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the value of the argument is not assignable to the specified type.
    ///     The exception message contains the argument name and the type information.
    /// </exception>
    /// <returns>The reference to the <paramref name="argument" /> object.</returns>
    public static ref readonly Guard.ArgumentInfo<Type> AssignableToOrNull(in this Guard.ArgumentInfo<Type> argument, Type type)
    {
        Guard.Argument(type, nameof(type)).NotNull();

        if (argument.Value != null && !type.IsAssignableFrom(argument.Value))
        {
            throw
                Guard.Fail(new
                               ArgumentException($"Instance of type specified in {argument.Name} - {argument.Value.FullName} cannot be assigned to an instance of {type.FullName}.",
                                                 argument.Name));
        }

        return ref argument;
    }

    /// <summary>
    ///     Checks if the specified type is assignable to the expected type.
    /// </summary>
    /// <typeparam name="TExpected">The expected type.</typeparam>
    /// <param name="argument">The argument to be checked.</param>
    /// <returns>
    ///     The original argument value if the type is assignable to the expected type;
    ///     otherwise, a default (null) value of <see cref="Guard.ArgumentInfo{Type}" />.
    /// </returns>
    public static ref readonly Guard.ArgumentInfo<Type> AssignableToOrNull<TExpected>(in this Guard.ArgumentInfo<Type> argument) =>
        ref AssignableToOrNull(argument, typeof(TExpected));
}
