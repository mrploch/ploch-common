using System;

namespace Ploch.Common.Tests.Reflection
{
    internal static class TestTypes
    {
        public class TestTypeWithMixedSettersAndGetter
        {
            public string? _stringPropNoGetter;

#pragma warning disable 649 //  [CS0649] Field 'TestTypes.TestTypeWithMixedSettersAndGetter._stringPropNoSetter' is never assigned to, and will always have its default value null
            public string? _stringPropNoSetter;
#pragma warning restore 649

            public int IntProp { get; set; }

            public string? StringProp { get; set; }

            public int IntProp2 { get; set; }

            public string? StringProp2 { get; set; }

            public string StringPropNoSetter => _stringPropNoSetter;

#pragma warning disable S2376 // Write-only properties should not be used
            public string StringPropNoGetter
#pragma warning restore S2376 // Write-only properties should not be used
            {
                set => _stringPropNoGetter = value;
            }
        }

        public class MyTestClass
        {
            public int IntProp { get; set; }

            public string? StringProp { get; set; }

            public string? StringProp2 { get; set; }

            public TestTypeBase? TestTypeBaseProp { get; set; }

            public TestType2? TestType2Prop { get; set; }

            protected string? MyProtectedStringProp { get; set; }
        }

        public class TestTypeBase
        { }

        public class TestType2 : TestTypeBase
        { }

        public class Class1
        {
            public string? MyProperty { get; set; }
        }

        [Attribute2(nameof(Attribute2Attribute))]
        public class ClassWith_Attribute2
        {
            public string? MyProperty { get; set; }
        }

        [Attribute1_1(PropInt = 100, PropInt2 = 200)]
        [Attribute2("Test2")]
        public class ClassWith_Attribute1_1_And_Attribute2
        {
            public string? MyProperty { get; set; }
        }

        [Attribute1("blah", PropInt = 111)]
        public class ClassWithInherited_Attribute1_1_And_Attribute2 : ClassWith_Attribute1_1_And_Attribute2
        {
            public string? MyProperty2 { get; set; }
        }

        public class Attribute1Attribute : Attribute
        {
            public Attribute1Attribute(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public int PropInt { get; set; }
        }

        public sealed class Attribute1_1Attribute : Attribute1Attribute
        {
            /// <inheritdoc />
            public Attribute1_1Attribute() : base(nameof(Attribute1_1Attribute))
            { }

            public int PropInt2 { get; set; }
        }

        public class Attribute2Attribute : Attribute
        {
            public Attribute2Attribute(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public int PropInt { get; set; }
        }
    }
}