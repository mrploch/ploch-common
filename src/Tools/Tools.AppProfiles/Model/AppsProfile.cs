using System.Collections.Generic;

namespace Ploch.Tools.AppProfiles.Model
{
    public record AppsProfile(string Name, ICollection<AppEntry> AppEntries);
}