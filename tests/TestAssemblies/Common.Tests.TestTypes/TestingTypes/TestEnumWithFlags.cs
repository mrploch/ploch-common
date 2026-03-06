using System;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

[Flags]
public enum TestEnumWithFlags
{
    None = 0,
    FirstValue = 1,
    SecondValue = 2,
    ThirdValue = 4,
    All = FirstValue | SecondValue | ThirdValue
}
