namespace Ploch.Common.Windows.Wmi;

public interface IWmiQuery : IDisposable
{
    IEnumerable<IWmiObject> Execute(string query);
}
