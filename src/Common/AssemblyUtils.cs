using System.Reflection;

namespace Ploch.Common
{
    public static class AssemblyUtils
    {
        public static Assembly GetAss()
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            var executingAssembly = Assembly.GetExecutingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();

            return entryAssembly;
        }
    }
}