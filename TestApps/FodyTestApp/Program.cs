using Ploch.TestApps.WorkerServiceApp;
using Serilog;
using System;
using System.Threading;

[assembly: Interceptor]

namespace FodyTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .MinimumLevel.Debug()
                         .WriteTo.Console()
                         .WriteTo.File("c:\\temp\\logfile2.log", rollingInterval: RollingInterval.Day)
                         .CreateLogger();

            Console.WriteLine("Hello World!");
            Log.Debug("Starting test class....");
            var c = new MyClass();
            while (true)
            {
                c.DoSomething(Guid.NewGuid().ToString(), 123);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
