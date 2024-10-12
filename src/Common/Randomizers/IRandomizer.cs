using Boolean = System.Boolean;

namespace Ploch.Common.Randomizers;

/// <summary>
/// A generic interface for random value generators.
/// </summary>
/// <typeparam name="TValue">The type of generated random values.</typeparam>
public interface IRandomizer<out TValue>
{
    /// <summary>
    /// Generates a random value of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <returns>A random value of type <typeparamref name="TValue"/>.</returns>
    TValue GetValue();
}