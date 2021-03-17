using System;
using System.Collections.Generic;

namespace Ploch.Common.Windows.SystemTools
{
    public class PowerShellResult
    {
        public string InformationOutput { get; }

        public string ErrorsOutput { get; }

        public string DiagnosticOutput { get; }
        
        public bool HadErrors { get; }

        public IEnumerable<Exception> Exceptions { get; }

        public PowerShellResult(string informationOutput, string errorsOutput, string diagnosticOutput, bool hadErrors, IEnumerable<Exception> exceptions)
        {
            InformationOutput = informationOutput;
            ErrorsOutput = errorsOutput;
            DiagnosticOutput = diagnosticOutput;
            HadErrors = hadErrors;
            Exceptions = exceptions;
        }
    }
}