using WmiLight;

namespace Ploch.Common.Windows.Wmi;

public class WindowsManagementProcessTools
{
    public void TerminateProcessByName(string processName)
    {
        // var connectionFactory = new DefaultConnectionFactory();
        using (var connection = new WmiConnection())
        {
            foreach (var process in connection.CreateQuery("SELECT * FROM Win32_Process"))
            {
                if (process.GetPropertyValue<string>("Name") == "cmd.exe")
                {
                    using (var terminateMethod = process.GetMethod("Terminate"))
                    using (var parameters = terminateMethod.CreateInParameters())
                    {
                        parameters.SetPropertyValue("Reason", 20);

                        var result = process.ExecuteMethod<uint>(terminateMethod, parameters, out var terminateOutParameters2);

                        if (result != 0)
                        {
                            throw new Exception($"Win32_Process::Terminate(...) failed with {result}");
                        }
                    }
                }
            }
        }
    }
}
