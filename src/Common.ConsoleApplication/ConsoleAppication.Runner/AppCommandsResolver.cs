using System;
using System.Linq;
using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Core;
using Validation;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public static class AppCommandsResolver
    {
        [Pure]
        public static Type GetArgumentsType([NotNull] Type applicationType)
        {
            Requires.NotNull(applicationType, nameof(applicationType));

            var interfaces = applicationType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)).ToArray();
            Requires.Argument(interfaces.Length == 1, nameof(applicationType),
                "applicationType should implement exactly one ICommand<TArgs> interface but it implements {}", interfaces.Length);
            var appInterface = interfaces[0];
            var genericArguments = appInterface.GetGenericArguments();
            Verify.Operation(genericArguments.Length == 1, "Expected a single generic argument on ICommand<> interface but found {}.", genericArguments.Length);
            return genericArguments[0];
        }
    }
}