using System.Reflection;

namespace Ploch.Common.Tests.TestAssembly1;

public static class TestAssembly1Info
{
    public static Assembly Assembly => typeof(TestAssembly1Info).Assembly;

    public static IDictionary<Type, IEnumerable<Type>> Inheritors => new Dictionary<Type, IEnumerable<Type>>
                                                                     {
                                                                     { typeof(BaseClass),
                                                                       [ typeof(DerivedClass), typeof(BaseClassImplementation1), typeof(DerivedClass) ] },
                                                                     { typeof(IBaseInterface),
                                                                       [ typeof(BaseClass),
                                                                         typeof(BaseClassImplementation1),
                                                                         typeof(DerivedClass),
                                                                         typeof(Interface1Implementation1),
                                                                         typeof(Interface1Implementation2) ] },
                                                                     { typeof(IGenericInterface<>),
                                                                       [ typeof(TestGenericClass3<>), typeof(TestClass4GenericClass3OfInt) ] } };
}
