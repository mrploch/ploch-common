using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ploch.Common
{
    public static class EnvironmentUtilities
    {
        public static string GetCurrentAppPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? AppDomain.CurrentDomain.BaseDirectory) ??
                   throw new InvalidOperationException("Could not get entry assembly name, one of the components was null");
        }

        public static IEnumerable<string> GetEnvironmentCommandLine(bool includeApplication = false)
        {
            var args = Environment.CommandLine.Split(' ');

            return includeApplication ? args : args.Skip(1);
        }
    }
}