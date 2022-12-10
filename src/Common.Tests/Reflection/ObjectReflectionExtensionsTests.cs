using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection
{
    public class ObjectReflectionExtensionsTests
    {
        [Theory]
        [AutoData]
        public void GetFieldValue_should_return_field_private_field_value(string privateFieldValue,
                                                                          int protectedFieldValue,
                                                                          Guid publicFieldValue,
                                                                          string privateStaticFieldValue)
        {
            var testType = new TestType(privateFieldValue, protectedFieldValue, publicFieldValue, privateStaticFieldValue);

            testType.GetFieldValue<string>("_privateField").Should().Be(privateFieldValue);
            testType.GetFieldValue<int>("ProtectedField").Should().Be(protectedFieldValue);
            testType.GetFieldValue<Guid>("PublicField").Should().Be(publicFieldValue);
            testType.GetFieldValue<string>("PrivateStaticField").Should().Be(privateStaticFieldValue);
        }

        private class TestType
        {
            private static string? PrivateStaticField;
            private readonly string _privateField;
            public readonly Guid PublicField;

            protected int ProtectedField;

            public TestType(string privateField, int protectedField, Guid publicField, string privateStaticFieldValue)
            {
                _privateField = privateField;
                ProtectedField = protectedField;
                PublicField = publicField;
                PrivateStaticField = privateStaticFieldValue;
            }
        }
    }
}