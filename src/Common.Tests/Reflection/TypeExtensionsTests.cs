using Xunit;
using Ploch.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Ploch.Common.Reflection.Tests
{
    public class TypeExtensionsTests
    {
        interface ITestInterface1 { }
        interface ITestInterface2 { }
        interface ITestInterface3 { }

        class TestClass1: ITestInterface1, ITestInterface2 { }

        interface ITestInterface4: ITestInterface1, ITestInterface2 { }

        class TestClass2: TestClass1 { }
        
        [Fact]
        public void IsImplementing_should_return_true_if_type_is_implementing_interface()
        {
            typeof(TestClass1).IsImplementing(typeof(ITestInterface1)).Should().BeTrue();
            typeof(TestClass1).IsImplementing(typeof(ITestInterface2)).Should().BeTrue();
            typeof(TestClass2).IsImplementing(typeof(ITestInterface1)).Should().BeTrue();
            typeof(TestClass2).IsImplementing(typeof(ITestInterface2)).Should().BeTrue();

            typeof(ITestInterface4).IsImplementing(typeof(ITestInterface1)).Should().BeTrue();
            typeof(ITestInterface4).IsImplementing(typeof(ITestInterface2)).Should().BeTrue();
        }

        [Fact]
        public void IsImplementing_should_return_false_if_type_is_not_implementing_interface()
        {
            typeof(TestClass1).IsImplementing(typeof(ITestInterface3)).Should().BeFalse();
            typeof(ITestInterface4).IsImplementing(typeof(ITestInterface3)).Should().BeFalse();
        }
        
        
    }
}