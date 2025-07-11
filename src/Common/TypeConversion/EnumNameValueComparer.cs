using System;
using System.Collections.Generic;

namespace Ploch.Common.TypeConversion;

public class EnumNameValueComparer : IEqualityComparer<EnumName>
{
    public bool Equals(EnumName? x, EnumName? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return (bool)y?.Name.IsNullOrEmpty()!;
        }

        if (y is null)
        {
            return x.Name.IsNullOrEmpty();
        }

        if (x.Name.IsNullOrEmpty())
        {
            return y.Name.IsNullOrEmpty();
        }

        return x.Name!.Equals(y.Name, x.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(EnumName obj)
    {
        if (obj is null)
        {
            return 0;
        }

        if (obj.Name is null)
        {
            return 0;
        }

        return obj.CaseSensitive ? obj.Name.GetHashCode() : obj.Name.ToUpperInvariant().GetHashCode();
    }
}
