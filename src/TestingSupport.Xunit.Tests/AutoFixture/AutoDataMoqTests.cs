// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoDataMoqTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Moq;
using Ploch.TestingSupport.TestTypes;
using Ploch.TestingSupport.Xunit.AutoFixture;
using Xunit;

namespace Ploch.TestinngSupport.Tests.AutoFixture
{
    public interface ITestInterface
    {
        string Name { get; set; }

        void DoSomething();

    }
    
    public class AutoDataMoqTests
    {
        [Theory, AutoDataMoq]
        public void Should_Create_Simple_types_Test(string str, SimpleTestTypes.Type1 testType, ITestInterface testInterface)
        {
            str.Should().NotBeEmpty();
            testType.Should().NotBeNull();
            testType.StringProperty1.Should().NotBeEmpty();

            testInterface.Should().NotBeNull();
            testInterface.Name.Should().NotBeEmpty();
            testInterface.Should().BeAssignableTo<IMocked>();

        }
    }
}