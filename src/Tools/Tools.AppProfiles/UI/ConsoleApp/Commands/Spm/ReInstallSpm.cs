// using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.Design;
// using System.Globalization;
// using System.IO;
// using System.Linq;
// using System.ServiceProcess;
// using System.Text.RegularExpressions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
// using Microsoft.SqlServer.Management.Smo;
// using Ploch.Common;
// using Ploch.Common.ConsoleApplication.Core;
//
// namespace Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.Spm
// {
//     public class ReInstallSpm : ICommand<ReinstallSpmArgs>
//     {
//         private readonly IOutput _output;
//         private readonly ILogger<ReInstallSpm> _logger;
//
//         public ReInstallSpm(IOutput output, ILogger<ReInstallSpm> logger)
//         {
//             _output = output;
//             _logger = logger;
//         }
//         /// <inheritdoc />
//         public void Execute(ReinstallSpmArgs options)
//         {
//             var server = new Server();
//             var db = server.Databases["SPM_master"];
//             db.Drop();
//
//             var namePattern = options.ServiceNamePattern.IsNullOrEmpty() ? ".*imanage.*spm.*" : options.ServiceNamePattern;
//             var services = ServiceController.GetServices().Where(srv => Regex.IsMatch(srv.ServiceName, namePattern));
//             if (!services.Any())
//             {
//                 _output.WriteErrorLine($"Couldn't find any service matching {namePattern}");
//                 throw new InvalidOperationException("Service not found.");
//             }
//
//             var service = services.Single();
//             _output.WriteLine($"Stopping service {service.DisplayName} ({service.ServiceName}");
//             if (service.Status == ServiceControllerStatus.Running)
//             {
//                 service.Stop();
//             }
//             
//
//             _output.WriteLine("Service stopped.");
//             var targetPath = Path.GetDirectoryName(options.TargetPath);
//             var targetFileName = Path.GetFileName(options.TargetPath);
//
//             var sourceFilePath = options.SourceLocation;
//
//
//
//             DirectoryInfo directory = new DirectoryInfo(targetPath);
//             File.Move(options.TargetPath, Path.Combine(targetPath, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture).Replace(':', '_').Replace("/", "_") + "_" + targetFileName + ".bak"));
//             File.Copy(options.SourceLocation, options.TargetPath);
//
//             //var files = directory.GetFileSystemInfos("*.jar");
//             //var file = files.Single();
//             
//
//
//         }
//     }
// }