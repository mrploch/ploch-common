using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Provides a scoped service wrapper that manages the lifetime of a service resolved from an <see cref="IServiceScope" />.
/// </summary>
public interface IScopedService : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Gets the resolved service instance.
    /// </summary>
    object Service { get; }
}

/// <summary>
///     Provides a strongly-typed scoped service wrapper that manages the lifetime of a service resolved from an <see cref="IServiceScope" />.
/// </summary>
/// <typeparam name="TService">The type of service to resolve.</typeparam>
public interface IScopedService<out TService> : IScopedService where TService : notnull
{
    /// <summary>
    ///     Gets the resolved service instance.
    /// </summary>
    new TService Service { get; }
}

/// <summary>
///     A scoped service wrapper that resolves a service from a dedicated <see cref="IServiceScope" />
///     and disposes the scope when the wrapper is disposed.
/// </summary>
public class ScopedService : IScopedService
{
    private readonly IServiceScope _scope;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScopedService" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to create a new scope.</param>
    /// <param name="serviceType">The type of service to resolve from the scope.</param>
    public ScopedService(IServiceProvider serviceProvider, Type serviceType)
    {
        _scope = serviceProvider.CreateScope();
        Service = _scope.ServiceProvider.GetRequiredService(serviceType);
    }

    /// <inheritdoc />
    public object Service { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Releases the resources used by the <see cref="ScopedService" />.
    /// </summary>
    /// <param name="disposing">
    ///     <see langword="true" /> to release both managed and unmanaged resources;
    ///     <see langword="false" /> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _scope.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    ///     Asynchronously releases the managed resources used by the <see cref="ScopedService" />.
    /// </summary>
    /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
        {
            return;
        }

        if (_scope is IAsyncDisposable scopeAsyncDisposable)
        {
            await scopeAsyncDisposable.DisposeAsync();
        }
        else
        {
            _scope.Dispose();
        }

        _disposed = true;
    }
}

/// <summary>
///     A strongly-typed scoped service wrapper that resolves a service of type <typeparamref name="TService" />
///     from a dedicated <see cref="IServiceScope" /> and disposes the scope when the wrapper is disposed.
/// </summary>
/// <typeparam name="TService">The type of service to resolve.</typeparam>
/// <param name="serviceProvider">The service provider used to create a new scope.</param>
public class ScopedService<TService>(IServiceProvider serviceProvider)
    : ScopedService(serviceProvider, typeof(TService)), IScopedService<TService>
    where TService : notnull
{
    /// <inheritdoc />
    public new TService Service => (TService)base.Service;
}
