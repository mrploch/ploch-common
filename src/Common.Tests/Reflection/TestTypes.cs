using System;

namespace Ploch.Common.Tests.Reflection
{
    internal static class TestTypes
    {
        public class TestTypeWithMixedSettersAndGetter
        {
            public string _stringPropNoGetter;
            public string _stringPropNoSetter;

            public int IntProp { get; set; }

            public string StringProp { get; set; }

            public int IntProp2 { get; set; }

            public string StringProp2 { get; set; }

            public string StringPropNoSetter => _stringPropNoSetter;

            public string StringPropNoGetter
            {
                set => _stringPropNoGetter = value;
            }
        }

        public class MyTestClass
        {
            public int IntProp { get; set; }

            public string StringProp { get; set; }

            public string StringProp2 { get; set; }

            public TestTypeBase TestTypeBaseProp { get; set; }

            public TestType2 TestType2Prop { get; set; }

            protected string MyProtectedStringProp { get; set; }
        }

        public class TestTypeBase
        { }

        public class TestType2 : TestTypeBase
        { }

        public class Class1
        {
            public string MyProperty { get; set; }
        }

        [Attribute2(nameof(Attribute2))]
        public class ClassWith_Attribute2
        {
            public string MyProperty { get; set; }
        }

        [Attribute1_1(PropInt = 100, PropInt2 = 200)]
        [Attribute2("Test2")]
        public class ClassWith_Attribute1_1_and_Attribute2
        {
            public string MyProperty { get; set; }
        }

        [Attribute1("blah", PropInt = 111)]
        public class ClassWithInherited_Attribute1_1_and_Attribute2 : ClassWith_Attribute1_1_and_Attribute2
        {
            public string MyProperty2 { get; set; }
        }

        public class Attribute1 : Attribute
        {
            public Attribute1(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public int PropInt { get; set; }
        }

        public sealed class Attribute1_1 : Attribute1
        {
            /// <inheritdoc />
            public Attribute1_1() : base(nameof(Attribute1_1))
            { }

            public int PropInt2 { get; set; }
        }

        public class Attribute2 : Attribute
        {
            public Attribute2(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public int PropInt { get; set; }
        }
    }
}