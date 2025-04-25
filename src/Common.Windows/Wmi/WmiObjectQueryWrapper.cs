using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public class WmiObjectQueryWrapper : IWmiQuery
{
    private readonly WmiConnection _connection;

    public WmiObjectQueryWrapper() : this(new WmiConnection())
    { }

    public WmiObjectQueryWrapper(WmiConnection connection) => _connection = connection;

    /// <summary>
    ///     Executes a WMI query and retrieves the resulting collection of WMI objects.
    /// </summary>
    /// <param name="query">
    ///     The WMI query string to execute. This should be a valid WMI query in the WQL (WMI Query Language) format.
    /// </param>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> of <see cref="IWmiObject" /> representing the WMI objects returned by the query.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="query" /> is <c>null</c>.
    /// </exception>
    public IEnumerable<IWmiObject> Execute(string query) => _connection.CreateQuery(query).Select(o => new WmiObjectWrapper(o));

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
