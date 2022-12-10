using System;
using System.Collections.Generic;
using FluentAssertions.Primitives;

namespace Ploch.TestingSupport.FluentAssertions
{
    public class EnumerableAssertions<T> : ReferenceTypeAssertions<IEnumerable<T>, EnumerableAssertions<T>>
    {
        public EnumerableAssertions(IEnumerable<T> subject) : base(subject)
        { }

        protected override string Identifier => throw new NotImplementedException();
    }
}