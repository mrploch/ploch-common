using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Ploch.Common.Windows;
using Ploch.Common.Windows.SystemTools;

namespace ServiceTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowsServiceInstaller.UninstallService("MicroFocusFAS-GoogleDriveProcessor");
            var success = WindowsServiceInstaller.InstallService("MicroFocusFAS-GoogleDriveProcessor",
                                                                    "Micro Focus FAS-Google Drive Processor",
                                                                    "Processes Google Drive items for Micro Focus FAS",
                                                                    @"C:\devmf\verity\current\master\data-mgmt-agent\AMM\Main\Agents\GoogleDrive\GoogleDriveProcessor\bin\Debug\GoogleDriveProcessor.exe",
                                                                    false,
                                                                    ".\\krzys",
                                                                    "prezI\"$7?",
                                                                    false);

            Console.WriteLine(success);
            Console.Read();
            var serviceControllers = ServiceController.GetServices(".");
            var service = serviceControllers.FirstOrDefault(sc => sc.ServiceName == "MicroFocusFAS-GoogleDriveProcessor");
            if (service == null)
            {
                Console.WriteLine("Installation failed");
                return;
            }
            Console.WriteLine(service.ServiceName);
            Console.WriteLine(service.DisplayName);
          
            Console.ReadLine();
        }
    }
}
