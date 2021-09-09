using Autofac;
using Dawn;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Ploch.Common.DependencyInjection.Autofac.Tests
{
    public interface IMyService
    { }

    public interface IMyOtherService
    { }

    public interface IMyComponent
    { }
    
    public class MyComponent1 : IMyComponent{}
    
    public class MyComponent2 : IMyComponent{}

    public interface IMyApp
    {
        IMyService MyService { get; }
        IMyOtherService MyOtherService { get; }
        
        IMyComponent MyComponent { get; }
    }

    public class MyService1 : IMyService
    { }

    public class MyService2 : IMyService
    { }

    public class MyOtherService1 : IMyOtherService
    { }

    public class MyApp : IMyApp
    {

        public MyApp(IMyService myService, IMyOtherService myOtherService, IMyComponent myComponent)
        {
            MyService = myService;
            MyOtherService = myOtherService;
            MyComponent = myComponent;
        }

        public IMyService MyService { get; }
        public IMyOtherService MyOtherService { get; }
        public IMyComponent MyComponent { get; }
    }
    
    public class MyServiceBundle : IServicesBundle
    {
        public void Configure([NotNull] IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMyService, MyService2>()
                             .AddTransient<IMyOtherService, MyOtherService1>()
                             .AddTransient<IMyApp, MyApp>();
            
        }
    }
    
    public class TestLoggingBundle : IServicesBundle
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestLoggingBundle([NotNull] ITestOutputHelper testOutputHelper)
        {
            
            _testOutputHelper = Guard.Argument(testOutputHelper, nameof(testOutputHelper)).NotNull().Value;
        }

        public void Configure([NotNull] IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.TestOutput(_testOutputHelper).CreateLogger();
            serviceCollection.AddLogging(builder => builder.AddSerilog());
            serviceCollection.AddTransient<IMyComponent, MyComponent2>();
        }
    }

    public class ServicesBundleModuleAdapterTests
    {
        private readonly ITestOutputHelper _output;

        public ServicesBundleModuleAdapterTests(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void Load_should_populate_container_builder()
        {
            var builder = new ContainerBuilder();
            var bundleAdapter = new ServiceBundlesModuleAdapter(new MyServiceBundle(), new TestLoggingBundle(_output));
            builder.RegisterModule(bundleAdapter);
            var sut = builder.Build();
            var logger = sut.Resolve<ILogger<ServicesBundleModuleAdapterTests>>();
            logger.LogInformation("Resolving IMyApp");
            var myApp = sut.Resolve<IMyApp>();
            logger.LogInformation("MyApp resolved: {@MyApp}", myApp);
            myApp.MyService.Should().NotBeNull().And.BeOfType<MyService2>(); 
            myApp.MyOtherService.Should().NotBeNull().And.BeOfType<MyOtherService1>();
            myApp.MyComponent.Should().NotBeNull().And.BeOfType<MyComponent2>();
            
            logger.LogInformation("Finished");

        }

        [Fact]
        public void Load_should_populate_container_builder_with_multiple_modules()
        {
            var builder = new ContainerBuilder();
            var bundleAdapter1 = new ServiceBundlesModuleAdapter(new MyServiceBundle());
            var bundleAdapter2 = new ServiceBundlesModuleAdapter(new TestLoggingBundle(_output));
            builder.RegisterModule(bundleAdapter1);
            builder.RegisterModule(bundleAdapter2);
            var sut = builder.Build();
            var logger = sut.Resolve<ILogger<ServicesBundleModuleAdapterTests>>();
            logger.LogInformation("Resolving IMyApp");
            var myApp = sut.Resolve<IMyApp>();
            logger.LogInformation("MyApp resolved: {@MyApp}", myApp);
            myApp.MyService.Should().NotBeNull().And.BeOfType<MyService2>();
            myApp.MyOtherService.Should().NotBeNull().And.BeOfType<MyOtherService1>();
            myApp.MyComponent.Should().NotBeNull().And.BeOfType<MyComponent2>();

            logger.LogInformation("Finished");

        }
    }
}