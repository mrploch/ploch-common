using FluentAssertions;
using Ploch.Common.IO;
using Xunit;

namespace Ploch.Common.Tests.IO
{
    public class PathUtilsTests
    {
        [Fact]
        public void GetDirectoryName_should_return_folder_name_in_provided_path()
        {
            PathUtils.GetDirectoryName(@"c:/myrootfolder/mysubfolder/expected-folder-name").Should().Be("expected-folder-name");
        }
    }
}