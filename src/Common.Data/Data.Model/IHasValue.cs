namespace Ploch.Common.Data.Model
{
    public interface IHasValue
    {
        object Value { get; set; }
    }
    public interface IHasValue<TValue> : IHasValue
    {
        new TValue Value { get; set; }
    }

    public interface IHasOptionalValue<TValue> : IHasValue<TValue?>
    {
        new TValue? Value { get; set; }
    }
}