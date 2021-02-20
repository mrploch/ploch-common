// unset

namespace Ploch.Common.WorkerServices
{
    public interface IServiceInstaller
    {
        void InstallService(string binPath, ServiceInfo serviceInfo = null);
    }
}