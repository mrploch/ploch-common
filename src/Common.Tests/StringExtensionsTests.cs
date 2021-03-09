using Xunit;
using Ploch.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ploch.TestingSupport.Xunit.AutoFixture;

namespace Ploch.Common.Tests
{
    public class StringExtensionsTests
    {
        [Theory, AutoDataMoq]
        public void IsNullOrEmptyTest(string str)
        {
            str.IsNullOrEmpty().Should().BeFalse();
            string nullString = null;
            nullString.IsNullOrEmpty().Should().BeTrue();

            "".IsNullOrEmpty().Should().BeTrue();
            
        }

        [Theory, AutoDataMoq]
        public void ToBase64String_should_correctly_encode(string str)
        {
            var base64String = str.ToBase64String(Encoding.UTF8);

            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            decoded.Should().Be(str);
        }
    }
}