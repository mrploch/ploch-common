using Ploch.Common.Collections;
using Ploch.Common.Windows.Wmi;
using Ploch.Common.Windows.Wmi.ManagementObjects;
using WmiLight;

namespace Ploch.Common.Windows.Tests;

public class WmiQueryTests
{
    [Fact]
    public void MyMethod()
    {
        using var con = new WmiConnection();
        foreach (var process in con.CreateQuery("SELECT * FROM Win32_Process"))
        {
            Console.WriteLine(process["Name"]);
            var propertyNames = process.GetPropertyNames();
            foreach (var propertyName in propertyNames)
            {
                Console.WriteLine($"{propertyName}: {process[propertyName]}");
            }

            Console.WriteLine();
        }
    }

    [Fact]
    public void GetServices()
    {
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());

        var services = queryFactory.Create().GetAll<WindowsManagementService>();

        var startModes = new HashSet<ServiceStartMode?>();
        var serviceTypes = new HashSet<string?>();
        var states = new HashSet<ServiceState?>();
        var startNames = new HashSet<string?>();
        var captionDisplayNamesDescriptions = new List<(string?, string?, string?)>();

        foreach (var service in services)
        {
            var processPathName = service.PathName;
            var processState = service.State;

            var processDescription = service.Description;
            service.DisplayName = service.DisplayName;

            startModes.Add(service.StartMode);
            serviceTypes.Add(service.ServiceType);
            states.Add(service.State);
            startNames.Add(service.StartName);
            captionDisplayNamesDescriptions.Add((service.Caption, service.DisplayName, processDescription));
            Console.WriteLine($"Service ID: {service.ProcessId}, Name: {service.Name}");
        }

        Console.WriteLine("using Ploch.Common.Windows.Wmi.ManagementObjects;");
        //  PrintStringsAsEnum(nameof(startModes), startModes);
        PrintStringsAsEnum(nameof(serviceTypes), serviceTypes);
        //  PrintStringsAsEnum(nameof(states), states);
        PrintStringsAsEnum(nameof(startNames), startNames);

        Console.WriteLine();
        Console.WriteLine("Caption, Display Name, Description");
        foreach (var (caption, displayName, description) in captionDisplayNamesDescriptions)
        {
            if (caption == displayName)
            {
                continue;
            }

            Console.WriteLine($"Caption: {caption}\nDisplay Name: {displayName}\nDescription: {description}");
            Console.WriteLine();
        }
    }

    private static void PrintStringsAsEnum(string setName, IEnumerable<string?> strings)
    {
        var mappings = new Dictionary<string, List<string?>>(StringComparer.OrdinalIgnoreCase);
        foreach (var str in strings)
        {
            var camelCase = !str.IsNullOrEmpty() ? str!.ToPascalCase() : "Empty";
            var str2 = str?.Replace(@"\", @"\\");
            if (mappings.ContainsKey(camelCase))
            {
                mappings[camelCase].Add(str2);
            }
            else
            {
                mappings[camelCase] = new List<string?> { str2 };
            }
        }

        Console.WriteLine($"public enum {setName.ToPascalCase()}");
        Console.WriteLine("{");

        foreach (var (enumEntry, enumValues) in mappings)
        {
            if ((enumValues.Count == 1 && enumValues[0] == null) ||
                (enumValues.Count == 1 && !enumValues[0].Equals(enumEntry, StringComparison.OrdinalIgnoreCase)) || enumValues.Count > 1)
            {
                Console.Write("    ");
                Console.WriteLine($"[WindowsManagementObjectEnumMapping({enumValues.Select(v => v != null ? $"\"{v}\"" : "null").Join(", ")})]");
            }

            Console.Write("    ");
            Console.WriteLine($"{enumEntry},");
        }

        Console.WriteLine("}");
        Console.WriteLine();
    }

    [Fact]
    public void GetProcesses()
    {
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());

        var processes = queryFactory.Create().GetAll<WindowsManagementProcess>();

        foreach (var process in processes)
        {
            Console.WriteLine($"Process ID: {process.ProcessId}, Name: {process.Name}");
        }
    }
}
