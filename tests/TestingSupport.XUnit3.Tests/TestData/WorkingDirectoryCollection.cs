using Xunit;

namespace Ploch.TestingSupport.XUnit3.Tests.TestData;

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
    public const string Name = "Working directory";
}
