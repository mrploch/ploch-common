using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection;

public interface IScopedService : IDisposable, IAsyncDisposable
{
    object Service { get; }
}

public interface IScopedService<out TService> : IScopedService where TService : notnull
{
    new TService Service { get; }
}

public class ScopedService : IScopedService
{
    private readonly IServiceScope _scope;

    public ScopedService(IServiceProvider serviceProvider, Type serviceType)
    {
        _scope = serviceProvider.CreateScope();
        Service = _scope.ServiceProvider.GetRequiredService(serviceType);
    }

    public object Service { get; }

    public void Dispose()
    {
        _scope.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_scope is IAsyncDisposable scopeAsyncDisposable)
        {
            await scopeAsyncDisposable.DisposeAsync();
        }
        else
        {
            _scope.Dispose();
        }
    }
}

public class ScopedService<TService> : ScopedService, IScopedService<TService> where TService : notnull
{
    public ScopedService(IServiceProvider serviceProvider) : base(serviceProvider, typeof(TService))
    { }

    public TService Service => (TService)base.Service;
}
