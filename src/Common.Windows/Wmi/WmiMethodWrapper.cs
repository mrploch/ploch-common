using System.Diagnostics;
using WmiLight;

namespace Ploch.Common.Windows.Wmi;

/// <summary>Represents a WMI defined method.</summary>
[DebuggerDisplay("{Name,nq} ({Class.Name,nq})")]
public class WmiMethodWrapper(WmiMethod wmiMethod) : IWmiMethod, IDisposable
{
    /// <summary>Gets the method name.</summary>
    public string Name => wmiMethod.Name;

    /// <summary>Gets the associates WMI class.</summary>
    public WmiClass Class => wmiMethod.Class;

    /// <summary>
    ///     <c>true</c> if the WMI method has any in parameter.
    /// </summary>
    public bool HasInParameters => wmiMethod.HasInParameters;

    /// <summary>
    ///     Creates an object representing the in parameters for a call of WMI this method.
    /// </summary>
    /// <returns>An object representing the in parameters for a call of WMI this method or <c>null</c> if no parameters are </returns>
    public IWmiMethodParameters CreateInParameters() => new WmiMethodParametersWrapper(wmiMethod.CreateInParameters());

    /// <summary>
    ///     Releases all resources used by the <see cref="T:WmiLight.WmiMethod" />.
    /// </summary>
    public void Dispose() => wmiMethod.Dispose();
}
