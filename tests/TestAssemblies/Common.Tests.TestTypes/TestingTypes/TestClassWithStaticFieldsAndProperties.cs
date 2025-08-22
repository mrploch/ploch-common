namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class TestClassWithStaticFieldsAndProperties
{
    public const string PrivateStaticPropName = nameof(PrivateStaticProp);
    public const string ProtectedStaticPropName = nameof(ProtectedStaticProp);
    public const string InternalStaticPropName = nameof(InternalStaticProp);
    public const string PublicStaticPropName = nameof(PublicStaticProp);
    public static string PrivateStaticField = "Private Static Property";
    public static string ProtectedStaticField = "Protected Static Property";
    public static string InternalStaticField = "Internal Static Property";

    public static string PublicStaticField = "Public Static Property";

    public static string PublicStaticProp { get; set; } = PublicStaticField;

    private static string PrivateStaticProp { get; set; } = PrivateStaticField;

    private static string ProtectedStaticProp { get; set; } = ProtectedStaticField;

    internal static string InternalStaticProp { get; set; } = InternalStaticField;
}
