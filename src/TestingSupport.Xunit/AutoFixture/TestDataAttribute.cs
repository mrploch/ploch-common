﻿using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;

namespace Ploch.TestingSupport.TestData
{
    public class TestDataProvider : ISpecimenBuilder
    {
        /// <summary>
        ///     Creates a new <see cref="T:System.DateTime" /> instance.
        /// </summary>
        /// <param name="request">The request that describes what to create.</param>
        /// <param name="context">Not used.</param>
        /// <returns>
        ///     A new <see cref="T:System.DateTime" /> instance, if <paramref name="request" /> is a request for a
        ///     <see cref="T:System.DateTime" />; otherwise, a <see cref="T:Ploeh.AutoFixture.Kernel.NoSpecimen" /> instance.
        /// </returns>
        public object Create(object request, ISpecimenContext context)
        {
            if (request is ParameterInfo parameter)
            {
                var testDataAttribute = parameter.GetCustomAttribute<TestDataAttribute>();
                if (testDataAttribute != null) return TestData.ReadText(testDataAttribute.DataFileName);
            }

            return new NoSpecimen();
        }
    }

    public class TestDataCustomization : ICustomization
    {
        /// <summary>
        ///     Customizes the specified fixture by adding the <see cref="T:System.Type" /> specific numeric sequence generators.
        /// </summary>
        /// <param name="fixture">The fixture to customize.</param>
        /// <exception cref="ArgumentNullException"><paramref name="" /> is <see langword="null" />.</exception>
        public void Customize(IFixture fixture)
        {
            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));
            fixture.Customizations.Add(new TestDataProvider());
        }
    }

    public class TestDataAttribute : CustomizeAttribute
    {
        public TestDataAttribute(string dataFileName)
        {
            DataFileName = dataFileName;
        }

        public string DataFileName { get; }

        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            return new TestDataCustomization();
        }
    }
}