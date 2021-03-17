using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace Ploch.Common.Windows.SystemTools
{
    public static class WindowsServiceInstaller
    {
        /// <summary>
        /// Installs the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="description">The description.</param>
        /// <param name="binaryPath">The binary path.</param>
        /// <param name="automaticStart">if set to <c>true</c> then the service startup type will be set to Automatic, Manual otherwise.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="password">The password.</param>
        /// <param name="removeExisting"><c>true</c> to delete existing service first</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public static bool InstallService(string serviceName, string displayName, string description, string binaryPath, bool automaticStart, string accountName, string password, bool removeExisting)
        {
            string scriptContent;
            var assembly = Assembly.GetAssembly(typeof(WindowsServiceInstaller));
            var resourceName = "Ploch.Common.Windows.SystemTools.Scripts.Install-Service.ps1";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException("Couldn't locate required resource")))
                {
                    scriptContent = reader.ReadToEnd();
                }
            }
            var result = PowerShellScriptRunner.RunScript(scriptContent, new Dictionary<string, object>
                                                                         {
                                                                             {"serviceName", serviceName},
                                                                             {"displayName", displayName },
                                                                             {"description", description },
                                                                             {"binaryPath", binaryPath },
                                                                             {"startupType", automaticStart ? "Automatic" : "Manual" },
                                                                             {"accountName", accountName },
                                                                             {"password", password },
                                                                             {"removeExisting", removeExisting }
                                                                         });
            if (result.HadErrors)
            {
                throw new ServiceInstallerException($"Failed to install {serviceName}", result.ErrorsOutput + result.InformationOutput + result.DiagnosticOutput, result.Exceptions);
            }
            return !result.HadErrors;
        }

        /// <summary>
        /// Removes the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns><c>true</c> if the service was removed</returns>
        /// <exception cref="ServiceInstallerException">Thrown when service removal failed.</exception>
        public static bool UninstallService(string serviceName)
        {
            var serviceController = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase));
            if (serviceController == null)
            {
                return false;
            }
            var process = new Process
                          {
                              StartInfo = new ProcessStartInfo("sc.exe", $"delete {serviceName}")
                                          {
                                              RedirectStandardOutput = true, 
                                              RedirectStandardError = true,
                                              UseShellExecute = false

                                          }
                          };
            process.Start();
            var completed = process.WaitForExit(2000);
            if (!completed || process.ExitCode != 0)
            {
                var output = NormalizeOutput(process.StandardOutput.ReadToEnd().Trim());
                var errorOutput = NormalizeOutput(process.StandardError.ReadToEnd());
                if (!string.IsNullOrEmpty(errorOutput))
                {
                    output += errorOutput;
                }
                throw new ServiceInstallerException($"Failed to remove service {serviceName} - {output}", process.ExitCode);
            }

            return true;
        }

        private static string NormalizeOutput(string output)
        {
            return output.Trim().Replace("\r", "").Replace("\n", "");
        }
    }
}