using System;
using System.Collections.Generic;

namespace Ploch.Common.WorkerServices.Windows
{
    public class SCWindowsServiceInstaller : IServiceInstaller
    {

        public void InstallService(string binPath, ServiceInfo serviceInfo)
        {
            
            throw new System.NotImplementedException();
        }
    }

    public enum InstallBehavior
    {
        RemoveOnFailure,
        IgnoreFailure,
        Throw
    }

    public class InstallationResult
    {
        public bool Success { get; }

        public IEnumerable<Exception> Exceptions { get; }

        public InstallationResult(bool success) : this(success, new List<Exception>())
        {
            
        }

        public InstallationResult(bool success, IEnumerable<Exception> exceptions)
        {
            
            Success = success;
            Exceptions = exceptions;
            
        }
    }
}