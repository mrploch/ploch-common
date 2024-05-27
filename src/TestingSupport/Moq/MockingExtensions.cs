using System;
using Moq;

namespace Ploch.TestingSupport.Moq
{
    /// <summary>
    /// This class provides an extension method for mocking objects.
    /// </summary>
    public static class MockingExtensions
    {
        /// <summary>
        /// Casts a <paramref name="mockedService"/> to a <see cref="Mock{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the mocked service.</typeparam>
        /// <param name="mockedService">The mocked service.</param>
        /// <returns>A <see cref="Mock{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when mockedService is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the provided object is not a mock.</exception>
        public static Mock<T> Mock<T>(this T mockedService)
            where T : class
        {
            if (mockedService == null)
            {
                throw new ArgumentNullException(nameof(mockedService));
            }

            var mocked = mockedService as IMocked<T>;
            if (mocked == null)
            {
                throw new InvalidOperationException($"Provided object ({mockedService.GetType().Name}) is not a mock!");
            }

            return mocked.Mock;
        }
    }
}