using WmiLight;

namespace Ploch.Common.Windows.Wmi;

/// <summary>Represents a WMI defined method.</summary>
public interface IWmiMethod : IDisposable
{
    /// <summary>Gets the method name.</summary>
    string Name { get; }

    /// <summary>Gets the associates WMI class.</summary>
    WmiClass Class { get; }

    /// <summary>
    ///     <c>true</c> if the WMI method has any in parameter.
    /// </summary>
    bool HasInParameters { get; }

    /// <summary>
    ///     Creates an object representing the in parameters for a call of WMI this method.
    /// </summary>
    /// <returns>An object representing the in parameters for a call of WMI this method or <c>null</c> if no parameters are </returns>
    IWmiMethodParameters CreateInParameters();
}

/// <summary>
///     Represents the in and out parameters of a WMI defined method.
/// </summary>
public interface IWmiMethodParameters : IDisposable
{
    /// <summary>
    ///     Not supported for method parameters.
    /// </summary>
    void Put();
}

/// <summary>
///     Represents the in and out parameters of a WMI defined method.
/// </summary>
public class WmiMethodParametersWrapper : IWmiMethodParameters
{
    private readonly WmiMethodParameters _wmiMethodParameters;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WmiMethodParametersWrapper" /> class.
    /// </summary>
    /// <param name="wmiMethodParameters">The WMI method parameters to wrap.</param>
    public WmiMethodParametersWrapper(WmiMethodParameters wmiMethodParameters) =>
        _wmiMethodParameters = wmiMethodParameters ?? throw new ArgumentNullException(nameof(wmiMethodParameters));

    /// <inheritdoc />
    public void Put() => _wmiMethodParameters.Put();

    /// <inheritdoc />
    public void Dispose() => _wmiMethodParameters.Dispose();

    /// <summary>
    ///     Implicit conversion from <see cref="WmiMethodParametersWrapper" /> to <see cref="IntPtr" />.
    /// </summary>
    /// <param name="parameters">The wrapper to convert.</param>
    /// <returns>The IntPtr representation of the underlying WmiMethodParameters.</returns>
    public static implicit operator IntPtr(WmiMethodParametersWrapper parameters) => parameters._wmiMethodParameters;
}
