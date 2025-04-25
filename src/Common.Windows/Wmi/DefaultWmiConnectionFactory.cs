using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public class DefaultWmiConnectionFactory : IWmiConnectionFactory
{
    public WmiConnection Create() => new();
}
