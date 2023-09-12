namespace Ploch.Common.UseCases;

/// <summary>
///     An asynchronous use case that returns a value.
/// </summary>
/// <typeparam name="TInput">The use case input type.</typeparam>
/// <typeparam name="TResult">The use case result type.</typeparam>
public interface IAsyncUseCase<in TInput, TResult>
{
    /// <summary>
    ///     Executes the use case asynchronously.
    /// </summary>
    /// <param name="input">The use case input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing an asynchronous operation.</returns>
    Task<TResult?> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}

/// <summary>
///     An asynchronous use case that does not return any value.
/// </summary>
/// <typeparam name="TInput">The use case input type.</typeparam>
public interface IAsyncUseCase<in TInput>
{
    /// <summary>
    ///     Executes the use case asynchronously.
    /// </summary>
    /// <param name="input">The use case input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing an asynchronous operation.</returns>
    Task ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}