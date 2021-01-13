namespace Ploch.Common.Data.Model
{
    public interface IHasValue<TValue>
    {
        TValue Value { get; set; }
    }
}