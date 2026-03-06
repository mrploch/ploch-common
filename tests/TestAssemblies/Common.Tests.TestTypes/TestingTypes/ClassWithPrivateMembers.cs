using System;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithPrivateMembers
{
    public const string PrivateFieldName = nameof(_privateField);
    public static string PrivateFieldValue = Guid.NewGuid().ToString();
    private string _privateField = PrivateFieldValue;
}
