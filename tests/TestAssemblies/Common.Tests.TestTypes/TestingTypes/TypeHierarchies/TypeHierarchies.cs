using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable InconsistentNaming

namespace Ploch.Common.Tests.Reflection;

#pragma warning disable CS8603
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class TypeHierarchies
{
    public static class HierarchyOne
    {
        public interface IBaseTestInterface
        { }

        public interface IDerivedTestInterface : IBaseTestInterface
        { }

        public class BaseInterfaceImplementor : IBaseTestInterface
        { }

        public class DerivedInterfaceImplementor : IDerivedTestInterface
        { }
    }

    public static class HierarchyTwo
    {
        // Test interface that no class in the assembly implements
        public interface INonImplementedInterface
        { }

        public interface IHierarchiesTestInterface
        { }

        public class ConcreteClass_IHierarchiesTestInterface : IHierarchiesTestInterface
        { }

        public abstract class AbstractClass_IHierarchiesTestInterface : IHierarchiesTestInterface
        { }
    }

    public static class HierarchyThree
    {
        // Test classes for abstract class inheritance
        public abstract class AbstractBaseClass
        { }

        public class ConcreteChildClass : AbstractBaseClass
        { }

        public abstract class AbstractChildClass : AbstractBaseClass
        { }
    }

    public static class HierarchyFour
    {
        // Test types for generic interface testing
        public interface IGenericTestInterface<T>
        { }

        public class GenericInterfaceImplementor : IGenericTestInterface<string>
        { }

        public class ConcreteGenericImplementor : IGenericTestInterface<int>
        { }
    }

    public static class HierarchyFive
    {
        // Test classes for this test
        public class BaseTypeClass
        { }

        public class DerivedTypeClass : BaseTypeClass
        { }
    }

    public static class HierarchySix
    {
        // Test types for nested type testing
        public interface INestedTypeInterface
        { }

        public class OuterClass
        {
            public class NestedImplementor : INestedTypeInterface
            { }

            public abstract class AbstractNestedImplementor : INestedTypeInterface
            { }
        }
    }

    public static class HierarchySeven
    {
        // Test interfaces and classes for this test
        public interface IContract
        { }

        public class ContractImplementor : IContract
        { }
    }

    public static class HierarchyEight
    {
        public interface IImplementation
        { }

        public class ImplementationImplementor : IImplementation
        { }
    }

    public static class HierarchyNine
    {
        // Test types for nested generic constraints testing
        // ReSharper disable once UnusedTypeParameter
        public interface INestedGenericInterface<T> where T : IEnumerable
        { }

        public class NestedGenericImplementor : INestedGenericInterface<List<string>>
        { }

        public class DifferentNestedGenericImplementor : INestedGenericInterface<Dictionary<string, int>>
        { }
    }

    public static class HierarchyTen
    {
        // Test classes for the class constraint test
        public class ReferenceClass
        { }

        public class DerivedReferenceClass : ReferenceClass
        { }
    }

    public class NonImplementingClass
    { }

    public class NonInheritingClass
    { }

    public class NonGenericImplementor
    { }
}
