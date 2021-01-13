using System;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands
{
    public class NameMatcher
    {
        private readonly ISet<string>? _names;

        public NameMatcher(IEnumerable<string>? names, bool caseSensitive = false)
        {
            _names = names == null || !names.Any() ? null : new HashSet<string>(names, caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        }

        public bool Matches(string name)
        {
            return _names == null || _names.Contains(name);
        }
    }
}