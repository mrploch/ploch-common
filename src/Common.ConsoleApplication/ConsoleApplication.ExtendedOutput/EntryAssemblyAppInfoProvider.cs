using System.Reflection;
using CG.Reflection.Services;

namespace Ploch.Common.ConsoleApplication.ExtendedOutput
{
    public class EntryAssemblyAppInfoProvider: IAppInfoProvider
    {
        private readonly IPackageService _packageService;

        public EntryAssemblyAppInfoProvider(IPackageService packageService)
        {
            _packageService = packageService;
        }

        /// <inheritdoc />
        public AppInfo GetAppInfo()
        {
            var entryAssembly = _packageService.EntryAssembly;
            return new AppInfo(entryAssembly.Title, entryAssembly.AssemblyVersion.ToString(), entryAssembly.Description, entryAssembly.Company, entryAssembly.Description);
        }
    }
}