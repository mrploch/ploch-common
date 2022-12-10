using System;
using System.Reflection;

namespace Ploch.Common
{
    public static class EnvironmentUtilities
    {
        public static string GetCurrentAppPath()
        {
            return Assembly.GetEntryAssembly()?.GetName().Name ??
                   throw new InvalidOperationException("Could not get entry assembly name, one of the components was null");
        }
    }
}