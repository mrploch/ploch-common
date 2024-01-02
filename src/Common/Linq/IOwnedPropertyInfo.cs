namespace Ploch.Common.Linq;

public interface IOwnedPropertyInfo
{
    object GetValue();

    void SetValue(object value);
}


public interface IOwnedPropertyInfo<out TType, TProperty>
{
    TType Owner { get; }

    TProperty GetValue();

    void SetValue(TProperty? value);
}