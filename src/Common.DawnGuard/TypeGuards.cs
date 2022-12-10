using System;
using Dawn;

namespace Ploch.Common.DawnGuard
{
    public static class TypeGuards
    {
        public static ref readonly Guard.ArgumentInfo<Type> AssignableTo(in this Guard.ArgumentInfo<Type> argument, Type type)
        {
            Guard.Argument(type, nameof(type)).NotNull();
            if (argument.Value == null) // Check whether the GUID is empty.
            {
                throw Guard.Fail(new ArgumentNullException(argument.Name, $"Argument {argument.Name} is null."));
            }

            return ref AssignableToOrNull(argument, type);
        }

        public static ref readonly Guard.ArgumentInfo<Type> AssignableTo<TExpected>(in this Guard.ArgumentInfo<Type> argument)
        {
            return ref AssignableTo(argument, typeof(TExpected));
        }

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

        public static ref readonly Guard.ArgumentInfo<Type> AssignableToOrNull<TExpected>(in this Guard.ArgumentInfo<Type> argument)
        {
            return ref AssignableToOrNull(argument, typeof(TExpected));
        }
    }
}