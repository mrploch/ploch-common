using System;
using Dawn;
using JetBrains.Annotations;
using Ploch.Common.ConsoleApplication.Runner;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.DependencyInjection.Unity
{
    public static class UnityAppBuilderExtensions
    {
        [NotNull]
        public static AppBuilder WithUnityContainer([NotNull] this AppBuilder appBuilder)
        {
            Guard.Argument(appBuilder, nameof(appBuilder)).NotNull();
            
            return appBuilder.WithServiceProvider(services => services.BuildServiceProvider());
        }

        [NotNull]
        public static AppBuilder WithUnityContainer([NotNull] this AppBuilder appBuilder, [NotNull] IUnityContainer container)
        {
            Guard.Argument(container, nameof(container)).NotNull();
            Guard.Argument(appBuilder, nameof(appBuilder)).NotNull();

            return appBuilder.WithServiceProvider(services => services.BuildServiceProvider(container));
        }

        public static AppBuilder WithUnityContainer(this AppBuilder appBuilder, Action<IUnityContainer> containerAction)
        {
            Guard.Argument(containerAction, nameof(containerAction)).NotNull();
            Guard.Argument(appBuilder, nameof(appBuilder)).NotNull();

            var container = new UnityContainer();
            containerAction(container);

            return appBuilder.WithUnityContainer(container);
        }

        [NotNull]
        public static AppBuilder WithUnityContainer([NotNull] this AppBuilder appBuilder, bool validateScopes)
        {
            Guard.Argument(appBuilder, nameof(appBuilder)).NotNull();

            return appBuilder.WithServiceProvider(services => services.BuildServiceProvider(validateScopes));
        }
    }
}