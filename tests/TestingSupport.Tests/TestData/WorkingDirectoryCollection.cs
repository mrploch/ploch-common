using Xunit;

namespace Ploch.TestingSupport.Tests;

/// <summary>
///     Groups every test that changes the process working directory so that xUnit never runs them alongside another
///     collection.
/// </summary>
/// <remarks>
///     <see cref="System.IO.Directory.SetCurrentDirectory" /> mutates process-wide state, so the guarantee that no other
///     test can observe the change has to be structural rather than a comment: collections in this assembly run in
///     parallel by default, and disabling parallelisation for this one is what actually enforces the invariant.
/// </remarks>
[CollectionDefinition(WorkingDirectoryCollection.Name, DisableParallelization = true)]
public sealed class WorkingDirectoryCollection
{
    /// <summary>
    ///     The collection name to put on <see cref="CollectionAttribute" />.
    /// </summary>
    /// <remarks>
    ///     Must stay a <c>const</c> rather than a static read-only property: both usages are attribute arguments, and
    ///     the C# specification requires those to be compile-time constants. It is <c>internal</c> because nothing
    ///     outside this test assembly consumes it.
    /// </remarks>
    internal const string Name = "Working directory";

    /// <summary>
    ///     Prevents instantiation - the type is a marker for xUnit's collection discovery and is never constructed.
    /// </summary>
    private WorkingDirectoryCollection()
    { }
}
