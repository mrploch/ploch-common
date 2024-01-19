using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Ploch.Common.Cryptography;
using Xunit;
using Xunit.Abstractions;

namespace Ploch.Common.Tests;

public class HashingTests
{
    private readonly ITestOutputHelper _outputHelper;

    public HashingTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void ToMD5HashString_should_generate_md5_hash_from_string()
    {
        using var memoryStream = GenerateTestStream();

        var hashString = memoryStream.ToMD5HashString();

        _outputHelper.WriteLine(hashString);
        hashString.Should().Be("50C441FBF40B655F4D9A0BE8A648B8D5");
    }

    [Fact]
    public void ToHashString_should_compute_hash_using_provided_algorithm()
    {
        using var memoryStream = GenerateTestStream();

        var hashString = memoryStream.ToHashString(SHA256.Create());
        hashString.Should().Be("605F0FDA4B735DEA45B9A38ACFD83A7F161F91EE2BA9E0E2071677640F0A08A8");
    }

    private static Stream GenerateTestStream()
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        var sb = new StringBuilder();
        for (var i = 0; i < 1000; i++)
        {
            sb.Append($"Test string {i}");
        }

        writer.Write(sb);

        memoryStream.Position = 0;

        return memoryStream;
    }
}