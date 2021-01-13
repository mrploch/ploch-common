using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ploch.Common;
using Ploch.Common.ConsoleApplication.Core;
using Ploch.Common.Windows;
using Formatting = Newtonsoft.Json.Formatting;

namespace Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.InstalledSoftware
{
    public class GetInstalledSoftware : ICommand<GetInstalledSoftwareArgs>
    {
        private readonly IOutput _output;
        private readonly ILogger<GetInstalledSoftware> _logger;

        public GetInstalledSoftware(IOutput output, ILogger<GetInstalledSoftware> logger)
        {
            _output = output;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Execute(GetInstalledSoftwareArgs options)
        {

            var data = InstalledApplications.GetUsingRegistry();
            var apps = data
                                .Where(root => root.Value.Count > 0)
                                .Where(root => 
                                        !root.Value.Any(pair => pair.Key == "SystemComponent" && 
                                                                pair.Value.ToString() == "1"))
                                    .ToDictionary(pair => pair.Value.ContainsKey("DisplayName") ? pair.Value["DisplayName"].ToString() : pair.Key, pair => pair.Value);
            var attributeCounter = new Dictionary<string, int>();

            foreach (var (keyName,contents) in apps)
            {
                _output.WriteLine().WriteLine("*************************************************************");
                _output.WriteLine(keyName).WriteLine("*************************************************************");
                foreach (var (key, value) in contents)
                {
                    if (!attributeCounter.ContainsKey(key))
                    {
                        attributeCounter[key] = 0;
                    }

                    attributeCounter[key]++;
                    _output.WriteLine($"{key} := {value}");
                }

                _output.WriteLine("*************************************************************").WriteLine();
            }

            var countDict = new Dictionary<string, object>();
            apps["attributeCounts"] = countDict;

            foreach (var (attributeName, count) in attributeCounter.OrderByDescending(kv => kv.Value))
            {
                _output.WriteLine($"{attributeName} count: {count}");
                countDict[attributeName] = count;
            }
            _output.WriteLine("*************************************************************").WriteLine();
            var targetPath
                = options.OutputPath.IsNullOrEmpty() ? Environment.CurrentDirectory : options.OutputPath;
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;

            using var streamWriter = new StreamWriter(Path.Combine(targetPath, "software.json"));
            using var jsonWriter = new JsonTextWriter(streamWriter);
            serializer.Serialize(jsonWriter, apps);
        }
    }
}