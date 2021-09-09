using System.Collections;
using System.Collections.Generic;
using FluentAssertions.Primitives;

namespace Ploch.TestingSupport.FluentAssertions
{
    public class EnumerableAssertions<T> : ReferenceTypeAssertions<IEnumerable<T>, EnumerableAssertions<T>>
    {
        protected override string Identifier => throw new System.NotImplementedException();

        public EnumerableAssertions(IEnumerable<T> subject) : base(subject)
        {
        }
    }
}