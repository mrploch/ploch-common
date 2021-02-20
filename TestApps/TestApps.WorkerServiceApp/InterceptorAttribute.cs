// unset

using MethodDecorator.Fody.Interfaces;
using Serilog;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Ploch.TestApps.WorkerServiceApp
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    public class InterceptorAttribute : Attribute, IMethodDecorator
    {
        private object _instance;

        private MethodBase _method;

        private object[] _args;

        // instance, method and args can be captured here and stored in attribute instance fields
        // for future usage in OnEntry/OnExit/OnException
        public void Init(object instance, MethodBase method, object[] args)
        {
            _instance = instance;
            _method = method;
            _args = args;
            Debug.WriteLine("--- init");
            
            Log.Debug($"Init: {method.DeclaringType.FullName + "." + method.Name} [{string.Join(',', args)}],");
        }

        public void OnEntry()
        {
            Debug.WriteLine("--- OnEntry");
            Log.Debug("OnEntry");
        }

        public void OnExit()
        {
            Debug.WriteLine("--- OnExit");
            Log.Debug("OnExit");
        }

        public void OnException(Exception exception)
        {
            Log.Debug(string.Format("OnException: {0}: {1}", exception.GetType(), exception.Message));
        }
    }
}