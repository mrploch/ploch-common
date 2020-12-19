using System;
using System.ComponentModel;
using Moq;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace Ploch.TestingSupport
{
    public class AutoMockingContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var strategy = new AutoMockingBuilderStrategy(Container);

            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }

        class AutoMockingBuilderStrategy : BuilderStrategy
        {
            private readonly IUnityContainer container;

            public AutoMockingBuilderStrategy(IUnityContainer container)
            {
                this.container = container;
            }

        //     public override void PreBuildUp(ref BuilderContext context)
        //     {
        //         
        //         var key = context.BuildComplete;
        //         context.Reso
        //         if (key.Type.IsInterface && !container.IsRegistered(key.Type))
        //         {
        //             context.Existing = CreateDynamicMock(key.Type);
        //         }
        //     }
        //
        //     private static object CreateDynamicMock(Type type)
        //     {
        //         var genericMockType = typeof(Mock<>).MakeGenericType(type);
        //         var mock = (Mock)Activator.CreateInstance(genericMockType);
        //         return mock.Object;
        //     }
         }
    }
}