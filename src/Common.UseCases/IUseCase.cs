namespace Ploch.Common.UseCases;

/// <summary>
///     A use case that returns a value.
/// </summary>
/// <typeparam name="TInput">The input type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
public interface IUseCase<in TInput, out TResult>
{
    /// <summary>
    ///     Executes the use case.
    /// </summary>
    /// <param name="input">The use case input value.</param>
    /// <returns>The execution result.</returns>
    TResult Execute(TInput input);
}

/// <summary>
///     A use case that does not return any value.
/// </summary>
/// <typeparam name="TInput">The input type.</typeparam>
public interface IUseCase<in TInput>
{
    /// <summary>
    ///     Executes the use case.
    /// </summary>
    /// <param name="input">The use case input value.</param>
    void Execute(TInput input);
}