/*
Copyright 2017 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace Ploch.Common.Windows.SystemTools
{
    /// <summary>
    ///     Applications helpers
    /// </summary>
    public static class InstalledApplications
    {
        /// <summary>
        ///     Gets the installed applications.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <param name="options">The options.</param>
        /// <returns>The installed applications.</returns>
        public static IEnumerable<string> GetUsingManagementClass(string machineName = "localhost", AuthenticationOptions options = null)
        {
            options = options ?? new AuthenticationOptions();
            var returnValues = new List<string>();
            var scope = SetScope(machineName, options);
            using (var cls = new ManagementClass(scope, new ManagementPath("StdRegProv"), null))
            {
               
                const uint localMachineRegistryKey = 0x80000002; // HKEY_LOCAL_MACHINE registry key code
                object[] args = {localMachineRegistryKey, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", null};
                cls.InvokeMethod("EnumKey", args);
                var keys = args[2] as string[];
                using (var methodParams = cls.GetMethodParameters("GetStringValue"))
                {
                    methodParams["hDefKey"] = localMachineRegistryKey;
                    if (keys != null)
                        foreach (var subKey in keys)
                        {
                            methodParams["sSubKeyName"] = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + subKey;
                            methodParams["sValueName"] = "DisplayName";
                            using (var results = cls.InvokeMethod("GetStringValue", methodParams, null))
                            {
                                if (results != null && (uint) results["ReturnValue"] == 0) returnValues.Add(results["sValue"].ToString());
                            }
                        }
                }
            }

            return returnValues;
        }

        public static Dictionary<string, Dictionary<string, object>> GetUsingRegistry()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                var subKeyNames = key.GetSubKeyNames();
                var distinctNames = subKeyNames.Distinct().ToArray();
                if (subKeyNames.Length != distinctNames.Length)
                {
                    throw new InvalidOperationException("Duplicate keys found under install branch in the registry!");
                }
                Console.WriteLine($"Number of items: {subKeyNames.Length}");
                var result = new Dictionary<string, Dictionary<string, object>>();

                foreach (string subkeyName in subKeyNames)
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        var allValues = new Dictionary<string, object>();
                        var keyNames = subkey.GetSubKeyNames();
                        Console.WriteLine($"keynames count: ${keyNames.Length}");
                        foreach (var keyName in keyNames)
                        {
                            Console.WriteLine($"Subkey: ${keyName}");
                        }
                        foreach (var valueName in subkey.GetValueNames())
                        {
                            var value = subkey.GetValue(valueName);
                            allValues.Add(valueName, value);
                        }
                        result.Add(subkeyName, allValues);
                    }
                }

                return result;
            }
        }

        /// <summary>
        ///     Sets the scope.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <param name="options">The options.</param>
        /// <returns>The management scope</returns>
        private static ManagementScope SetScope(string machineName, AuthenticationOptions options)
        {
            if (options.Impersonate)
                return new ManagementScope(@"\\" + machineName + @"\root\default",
                    new ConnectionOptions {EnablePrivileges = true, Impersonation = ImpersonationLevel.Impersonate});
            if (!string.IsNullOrEmpty(options.UserName))
                return new ManagementScope(@"\\" + machineName + @"\root\default",
                    new ConnectionOptions {Username = options.UserName, Password = options.Password});
            return new ManagementScope(@"\\" + machineName + @"\root\default");
        }
    }
}