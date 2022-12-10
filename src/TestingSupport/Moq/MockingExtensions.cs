using System;
using Moq;

namespace Ploch.TestingSupport.Moq
{
    public static class MockingExtensions
    {
        public static Mock<T> Mock<T>(this T mockedService) where T : class
        {
            if (mockedService == null)
            {
                throw new ArgumentNullException("mockedService");
            }

            var mocked = mockedService as IMocked<T>;
            if (mocked == null)
            {
                throw new InvalidOperationException(string.Format("Provided object ({0}) is not a mock!", mockedService.GetType().Name));
            }

            return mocked.Mock;
        }
    }
}