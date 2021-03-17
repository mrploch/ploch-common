using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Ploch.Common.Windows.SystemTools
{
    public static class PowerShellScriptRunner
    {
        /// <summary>
        /// Runs a PowerShell script with parameters and prints the resulting pipeline objects to the console output. 
        /// </summary>
        /// <param name="scriptContents">The script file contents.</param>
        /// <param name="scriptParameters">A dictionary of parameter names and parameter values.</param>
        public static PowerShellResult RunScript(string scriptContents, Dictionary<string, object> scriptParameters)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript(scriptContents);

                ps.AddParameters(scriptParameters);
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                var diagnosticBuilder = new StringBuilder();
                var exceptions = new List<Exception>();

                ps.Streams.Debug.DataAdded += (sender, args) => OnDataAdded<DebugRecord>(sender, args, "DebugStream", diagnosticBuilder, record => record.Message);
                ps.Streams.Error.DataAdded += (sender, args) => OnDataAdded<ErrorRecord>(sender,
                                                                                         args,
                                                                                         "ErrorStream",
                                                                                         errorBuilder,
                                                                                         record =>
                                                                                         {
                                                                                             if (record.Exception != null)
                                                                                             {
                                                                                                 exceptions.Add(record.Exception);
                                                                                             }
                                                                                             return record;
                                                                                         });
                ps.Streams.Information.DataAdded += (sender, args) =>
                                                        OnDataAdded<InformationRecord>(sender,
                                                                                       args,
                                                                                       "InformationStream",
                                                                                       outputBuilder,
                                                                                       record => record);
                ps.Streams.Progress.DataAdded += (sender, args) =>
                                                     OnDataAdded<ProgressRecord>(sender, args, "ProgressStream", outputBuilder, record => record);
                ps.Streams.Verbose.DataAdded += (sender, args) =>
                                                    OnDataAdded<VerboseRecord>(sender, args, "VerboseStream", diagnosticBuilder, record => record.Message);
                ps.Streams.Warning.DataAdded += (sender, args) =>
                                                    OnDataAdded<WarningRecord>(sender, args, "WarningStream", outputBuilder, record => record.Message);

                // execute the script and await the result.
                var pipelineObjects = ps.Invoke();
                
                var hadErrors = ps.HadErrors;
                outputBuilder.AppendLine($"Had Errors? {hadErrors}");

                foreach (var item in pipelineObjects)
                {
                    outputBuilder.AppendLine(item.BaseObject.ToString());
                }

                return new PowerShellResult(outputBuilder.ToString(),
                                            errorBuilder.ToString(),
                                            diagnosticBuilder.ToString(),
                                            hadErrors,
                                            exceptions);
            }
        }
        
        private static void OnDataAdded<TRecord>(object sender, DataAddedEventArgs e, string prefix, StringBuilder builder, Func<TRecord, object> messageFunc)
        {
            if (sender is PSDataCollection<TRecord> streamObjectsReceived)
            {
                var currentStreamRecord = streamObjectsReceived[e.Index];

                builder.AppendLine($"{prefix}: {messageFunc(currentStreamRecord)}");
            }
        }
    }
}