using FluentAssertions;
using Ploch.TestingSupport.TestData;
using Xunit;

namespace Ploch.TestingSupport.Tests.TestData
{
    public class RandomDataTests
    {
        [Fact]
        public void Generate_should_create_random_text_of_specified_length()
        {
            var str1 = RandomData.GenerateString(200);
            str1.Should().HaveLength(200);
        }
    }
}