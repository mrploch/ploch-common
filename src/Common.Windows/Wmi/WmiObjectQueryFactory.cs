namespace Ploch.Common.Windows.Wmi;

public class WmiObjectQueryFactory(IWmiConnectionFactory connectionFactory) : IWmiObjectQueryFactory
{
    public IWmiQuery Create() => new WmiObjectQueryWrapper(connectionFactory.Create());
}
