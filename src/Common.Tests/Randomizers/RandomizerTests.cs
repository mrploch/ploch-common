using FluentAssertions;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizers
{
    public class RandomizerTests
    {
        [Fact]
        public void GetRandomizer_for_not_supported_type_should_throw_NotSupportedException()
        {
            var execute = () => Randomizer.GetRandomizer<object>();

            execute.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void GetRandomizer_for_boolean_should_return_random_value_each_time()
        {
            var initialValue = Randomizer.GetRandomizer<bool>().GetValue();
            for (var i = 0; i < 5; i++)
            {
                if (Randomizer.GetRandomizer<bool>().GetValue() != initialValue)
                {
                    return;
                }
            }

            Assert.Fail("Randomizer returned the same value multiple times");
        }
    }
}