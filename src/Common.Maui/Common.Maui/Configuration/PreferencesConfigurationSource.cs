using Microsoft.Maui.Storage;
using Microsoft.Extensions.Configuration;

namespace Ploch.Common.Maui.Configuration;
public class PreferencesConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => throw new NotImplementedException();
}

public class PreferencesConfigurationProvider(IPreferences preferences) : ConfigurationProvider
{
    public override void Load() => throw new NotImplementedException();

    public override bool TryGet(string key, out string? value)
    {
        value = null;
        if (!preferences.ContainsKey(key))
        {
            return false;
        }

        value = preferences.Get(key, string.Empty);

        return true;
    }

    public override void Set(string key, string? value) => preferences.Set(key, value);

    public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath) => base.GetChildKeys(earlierKeys, parentPath);
}
