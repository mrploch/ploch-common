namespace Ploch.Common.Matchers;

public interface IStringMatcher : IMatcher<string?>
{
    bool IsMatch(string? value);
}
