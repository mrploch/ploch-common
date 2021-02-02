using System;
using System.Linq;
using Ardalis.GuardClauses;
using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public static class AppCommandsResolver
    {
        [Pure]
        public static Type GetArgumentsType([NotNull] Type applicationType)
        {
            Guard.Against.Null(applicationType, nameof(applicationType));

            var interfaces = applicationType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)).ToArray();
            Guard.Against.ArgCondition(interfaces.Length != 1, nameof(applicationType), $"applicationType should implement exactly one ICommand<TArgs> interface but it implements {interfaces.Length}");
            
            var appInterface = interfaces[0];
            var genericArguments = appInterface.GetGenericArguments();
            Guard.Against.OperationCondition(genericArguments.Length != 1, $"Expected a single generic argument on ICommand<> interface but found {genericArguments.Length}.");
            return genericArguments[0];
        }
    }
}