using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

public class PathGuardTests
{
    [Fact]
    public void EnsureFileExists_should_throw_InvalidOperationException_for_non_existing_file()
    {
        var randomFileName = $@"c:\{Guid.NewGuid()}\{Guid.NewGuid()}.txt";

        var act = () => randomFileName.EnsureFileExists(nameof(randomFileName));

        act.Should().Throw<ArgumentException>();
    }
}
