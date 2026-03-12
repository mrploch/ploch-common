using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Ploch.Common.DependencyInjection.Tests;

public class ScopedServiceTests
{
    [Fact]
    public void Constructor_should_resolve_service_from_new_scope()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        using var scopedService = new ScopedService(provider, typeof(ITestService));

        scopedService.Service.Should().BeOfType<TestService>();
    }

    [Fact]
    public void Dispose_should_dispose_scope()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService(provider, typeof(ITestService));
        var resolvedService = (TestService)scopedService.Service;

        scopedService.Dispose();

        resolvedService.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_should_be_idempotent()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService(provider, typeof(ITestService));

        scopedService.Dispose();

        // Second dispose should not throw
        var act = () => scopedService.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public async Task DisposeAsync_should_dispose_scope()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService(provider, typeof(ITestService));
        var resolvedService = (TestService)scopedService.Service;

        await scopedService.DisposeAsync();

        resolvedService.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public async Task DisposeAsync_should_be_idempotent()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService(provider, typeof(ITestService));

        await scopedService.DisposeAsync();

        // Second dispose should not throw
        var act = async () => await scopedService.DisposeAsync();
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DisposeAsync_after_Dispose_should_not_throw()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService(provider, typeof(ITestService));

        scopedService.Dispose();

        // DisposeAsync after Dispose should be safe (idempotent)
        var act = async () => await scopedService.DisposeAsync();
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Dispose_after_DisposeAsync_should_not_throw()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService(provider, typeof(ITestService));

        await scopedService.DisposeAsync();

        // Dispose after DisposeAsync should be safe (idempotent)
        var act = () => scopedService.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public async Task GenericScopedService_should_dispose_scope_async()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService<ITestService>(provider);
        var resolvedService = (TestService)scopedService.Service;

        await scopedService.DisposeAsync();

        resolvedService.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void GenericScopedService_should_resolve_typed_service()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        using var scopedService = new ScopedService<ITestService>(provider);

        scopedService.Service.Should().BeOfType<TestService>();
    }

    [Fact]
    public void GenericScopedService_should_dispose_scope()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var scopedService = new ScopedService<ITestService>(provider);
        var resolvedService = (TestService)scopedService.Service;

        scopedService.Dispose();

        resolvedService.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public async Task DisposeAsync_should_fall_back_to_sync_dispose_when_scope_is_not_async_disposable()
    {
        // Create a mock scope that implements IDisposable but NOT IAsyncDisposable
        var mockScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(p => p.GetService(typeof(ITestService)))
                           .Returns(new TestService());
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

        var outerProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);
        outerProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory)))
                     .Returns(mockScopeFactory.Object);

        var scopedService = new ScopedService(outerProvider.Object, typeof(ITestService));

        await scopedService.DisposeAsync();

        mockScope.Verify(s => s.Dispose(), Times.Once);
    }

    private interface ITestService
    {
    }

    private sealed class TestService : ITestService, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
