using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public interface IWmiObject
{
    /// <summary>
    ///     Gets the value that is used to distinguish between classes and instances.
    /// </summary>
    WmiObjectGenus Genus { get; }

    /// <summary>
    ///     Gets the Class name.
    /// </summary>
    string Class { get; }

    /// <summary>
    ///     Gets the name of the immediate parent class of the class or instance.
    /// </summary>
    string SuperClass { get; }

    /// <summary>
    ///     Gets the name of the top-level class from which the class or instance is derived.
    ///     <para />
    ///     When this class or instance is the top-level class, the values of <see cref="Dynasty" /> property and the <see cref="Class" /> property are the same.
    /// </summary>
    string Dynasty { get; }

    /// <summary>
    ///     Gets the relative path to the class or instance.
    /// </summary>
    string Namespace { get; }

    /// <summary>
    ///     Gets the full path to the class or instance—including server and namespace.
    /// </summary>
    /// <summary>
    ///     Gets a particular property value.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The particular property value.</returns>
    object? this[string propertyName] { get; }


    IEnumerable<(string, object?)> GetProperties();

    WmiObject GetWmiObject();

    /// <summary>
    ///     Gets a WMI method for this class.
    /// </summary>
    /// <param name="methodName">The WMI method name.</param>
    /// <returns>The requested Method.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="methodName" /> is null.</exception>
    IWmiMethod GetMethod(string methodName);

    /// <summary>
    ///     Gets the names of all properties in the object.
    /// </summary>
    /// <returns>The names of all properties.</returns>
    IEnumerable<string> GetPropertyNames();

    /// <summary>
    ///     Gets a particular property value.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The particular property value.</returns>
    object? GetPropertyValue(string propertyName);

    /// <summary>
    ///     Gets a particular property value.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The particular property value.</returns>
    TResult? GetPropertyValue<TResult>(string propertyName);


    /// <summary>
    ///     Executes a static WMI method.
    /// </summary>
    /// <param name="method">The method that should be executed.</param>
    /// <param name="outParameters">The out parameters returned by the method.</param>
    /// <returns>The return value of the method.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="method" /> parameter is <c>null</c>.</exception>
    object ExecuteMethod(WmiMethod method, out WmiMethodParameters outParameters);

    /// <summary>
    ///     Executes a static WMI method.
    /// </summary>
    /// <typeparam name="TReturnValue">The return value type.</typeparam>
    /// <param name="method">The method that should be executed.</param>
    /// <param name="outParameters">The out parameters returned by the method.</param>
    /// <returns>The return value of the method.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="method" /> parameter is <c>null</c>.</exception>
    TReturnValue ExecuteMethod<TReturnValue>(WmiMethod method, out WmiMethodParameters outParameters);

    /// <summary>
    ///     Executes a static WMI method.
    /// </summary>
    /// <param name="method">The method that should be executed.</param>
    /// <param name="inParameters">The parameters for the method.</param>
    /// <param name="outParameters">The out parameters returned by the method.</param>
    /// <returns>The return value of the method.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="method" /> parameter is <c>null</c>.</exception>
    object ExecuteMethod(WmiMethod method, WmiMethodParameters inParameters, out WmiMethodParameters outParameters);

    /// <summary>
    ///     Executes a static WMI method.
    /// </summary>
    /// <typeparam name="TReturnValue">The return value type.</typeparam>
    /// <param name="method">The method that should be executed.</param>
    /// <param name="inParameters">The parameters for the method.</param>
    /// <param name="outParameters">The out parameters returned by the method.</param>
    /// <returns>The return value of the method.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="method" /> parameter is <c>null</c>.</exception>
    TReturnValue ExecuteMethod<TReturnValue>(WmiMethod method, WmiMethodParameters inParameters, out WmiMethodParameters outParameters);
}
