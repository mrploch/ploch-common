using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Tests.Collections
{
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public class CollectionExtensionsTests
    {
        [Fact]
        public void KeyValuePairAdd()
        {
            var list = new List<KeyValuePair<string, int>>();

            list.Add("test1", 1);
            list.Add("test2", 2).Add("test3", 3);
            list.Should().HaveCount(3);

            list.Should()
                .Contain(new List<KeyValuePair<string, int>>
                {
                    new KeyValuePair<string, int>("test1", 1), new KeyValuePair<string, int>("test2", 2), new KeyValuePair<string, int>("test3", 3)
                });
        }

        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        [Fact]
        public void KeyValuePairThrowsOnNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                List<KeyValuePair<string, string>> list = null;
                list.Add("a", "b");
            });
        }
    }
}