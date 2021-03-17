using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ploch.Common.Windows.SystemTools
{
    /// <summary>
    /// Exception thrown when <see cref="WindowsServiceInstaller"/> operation fails.
    /// </summary>
    [Serializable]
    public class ServiceInstallerException : Exception
    {
        public string FailureDetails { get; }
        public IEnumerable<Exception> Exceptions { get; }
        public int? ErrorCode { get; }

        public ServiceInstallerException()
        {
        }

        protected ServiceInstallerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceInstallerException(string message) : base(message)
        {
        }

        public ServiceInstallerException(string message, string failureDetails, IEnumerable<Exception> exceptions) : base(message)
        {
            FailureDetails = failureDetails;
            Exceptions = exceptions;
        }

        public ServiceInstallerException(string message, int errorCode) : this(message, null, errorCode)
        {
        }

        public ServiceInstallerException(string message, string failureDetails, int errorCode) : base(message)
        {
            FailureDetails = failureDetails;
            ErrorCode = errorCode;
        }

        public ServiceInstallerException(string message, Exception innerException) : this(message, null, innerException)
        {
        }

        public ServiceInstallerException(string message, string failureDetails, Exception innerException) : base(message, innerException)
        {
            FailureDetails = failureDetails;
        }

        public ServiceInstallerException(string message, int errorCode, Exception innerException) : this(message, null, errorCode, innerException)
        {
        }

        public ServiceInstallerException(string message, string failureDetails, int errorCode, Exception innerException) : base(message, innerException)
        {
            FailureDetails = failureDetails;
            ErrorCode = errorCode;
        }
    }
}