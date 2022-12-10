namespace Ploch.TestingSupport.TestTypes
{
    public static class SimpleTestTypes
    {
        public class Type1
        {
            public int IntProperty1 { get; set; }

            public string StringProperty1 { get; set; }

            public bool BoolProperty1 { get; set; }

            /// <inheritdoc />
            public override string ToString()
            {
                return $@"{nameof(IntProperty1)}: {IntProperty1}, {nameof(StringProperty1)}: {StringProperty1}, {nameof(BoolProperty1)}: {BoolProperty1}";
            }
        }
    }
}