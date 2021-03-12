using Dawn;
using System;
using System.Linq;
using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public static class AppCommandsResolver
    {
        [Pure]
        public static Type GetArgumentsType([NotNull] Type applicationType)
        {
            Guard.Argument(applicationType, nameof(applicationType)).NotNull();

            Type commandType = typeof(ICommand<>);
            var interfaces = applicationType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == commandType).ToArray();
            Guard.Argument(applicationType).Require(interfaces.Length == 1, t => $"applicationType should implement exactly one ICommand<TArgs> interface but it implements {interfaces.Length}");
           // Guard.Against.ArgCondition(interfaces.Length != 1, nameof(applicationType), $"applicationType should implement exactly one ICommand<TArgs> interface but it implements {interfaces.Length}");
            
            var appInterface = interfaces[0];
            var genericArguments = appInterface.GetGenericArguments();
            Guard.Argument(applicationType).Require(genericArguments.Length != 1, t => $"Expected a single generic argument on ICommand<> interface but found {genericArguments.Length}.");
            return genericArguments[0];
        }
    }
}