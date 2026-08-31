using System;
using System.IO;

namespace Ploch.TestingSupport.TestData;

/// <summary>
///   Resolves the path of a test data file supplied to a data attribute.
/// </summary>
internal static class TestDataFilePathResolver
{
  /// <summary>
  ///   Resolves <paramref name="filePath" /> to an absolute path.
  /// </summary>
  /// <remarks>
  ///   A relative path is anchored to <see cref="AppContext.BaseDirectory" /> - the directory the consuming test
  ///   assembly was loaded from, which is where content files marked <c>CopyToOutputDirectory</c> are placed. Anchoring
  ///   to the process working directory instead would make data loading depend on how the test host happened to be
  ///   launched. A rooted path is returned unchanged apart from normalisation.
  /// </remarks>
  /// <param name="filePath">The absolute or relative path to the test data file.</param>
  /// <returns>The normalised absolute path to the test data file.</returns>
  public static string Resolve(string filePath)
  {
    var rootedPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(AppContext.BaseDirectory, filePath);

    return Path.GetFullPath(rootedPath);
  }
}
