using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public interface IWmiConnectionFactory
{
    WmiConnection Create();
}
