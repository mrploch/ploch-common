using System;
using Dawn;

namespace Ploch.Common.DawnGuard
{
    /// <summary>
    /// Provides utility methods for type guards.
    /// </summary>
    public static class TypeGuards
    {
        /// <summary>
        /// Checks if the provided type is assignable to the specified type.
        /// </summary>
        /// <param name="argument">The type to check.</param>
        /// <param name="type">The type to which the argument should be assignable.</param>
        /// <returns>The original argument info for method chaining.</returns>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        public static ref readonly Guard.ArgumentInfo<Type> AssignableTo(in this Guard.ArgumentInfo<Type> argument, Type type)
        {
            Guard.Argument(type, nameof(type)).NotNull();
            if (argument.Value == null) // Check whether the GUID is empty.
            {
                throw Guard.Fail(new ArgumentNullException(argument.Name, $"Argument {argument.Name} is null."));
            }

            return ref AssignableToOrNull(argument, type);
        }

        /// <summary>
        /// Checks if the provided type is assignable to the type of <typeparamref name="TExpected"/>.
        /// </summary>
        /// <param name="argument">The type to check.</param>
        /// <returns>The original argument info for method chaining.</returns>
        public static ref readonly Guard.ArgumentInfo<Type> AssignableTo<TExpected>(in this Guard.ArgumentInfo<Type> argument)
        {
            return ref AssignableTo(argument, typeof(TExpected));
        }

        /// <summary>
        /// Checks if the value of the argument is assignable to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The reference to the argument object.</param>
        /// <param name="type">The type to check against.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the value of the argument is not assignable to the specified type.
        /// The exception message contains the argument name and the type information.
        /// </exception>
        /// <returns>The reference to the argument object.</returns>
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
        /// Checks if the specified type is assignable to the expected type.
        /// </summary>
        /// <typeparam name="TExpected">The expected type.</typeparam>
        /// <param name="argument">The argument to be checked.</param>
        /// <returns>
        /// The original argument value if the type is assignable to the expected type;
        /// otherwise, a default (null) value of <see cref="Guard.ArgumentInfo{Type}"/>.
        /// </returns>
        public static ref readonly Guard.ArgumentInfo<Type> AssignableToOrNull<TExpected>(in this Guard.ArgumentInfo<Type> argument)
        {
            return ref AssignableToOrNull(argument, typeof(TExpected));
        }
    }
}