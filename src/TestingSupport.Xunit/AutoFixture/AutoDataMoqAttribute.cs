﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using AutoFixture.Xunit2;
using Ploch.TestingSupport.AutoFixture;

namespace Ploch.TestingSupport.Xunit.AutoFixture
{
    public class AutoDataMoqAttribute : AutoDataAttribute
    {
        public AutoDataMoqAttribute()
            : base(FixtureFactory.CreateFixture)
        { }
    }
}