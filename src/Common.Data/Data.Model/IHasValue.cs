namespace Ploch.Common.Data.Model
{
    public interface IHasValue<TValue>
    {
        TValue Value { get; set; }
    }

    public interface IHasOptionalValue<TValue> : IHasValue<TValue?>
    {
        new TValue? Value { get; set; }
    }
}