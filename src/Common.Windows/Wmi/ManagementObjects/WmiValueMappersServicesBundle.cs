using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;
using Ploch.Tools.SystemProfiles.Utilities.Windows;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

public class WmiValueMappersServicesBundle : ServicesBundle
{
    public override void DoConfigure()
    {
        AddConverter<EnumConverter>().AddConverter<DefaultManagementObjectTypeConverter>().AddConverter<DateTimeConverter>();
    }

    private WmiValueMappersServicesBundle AddConverter<TConverter>()
        where TConverter : class, IManagementObjectTypeConverter
    {
        Services.AddSingleton<TConverter>().AddKeyedSingleton<IManagementObjectTypeConverter, TConverter>(typeof(TConverter).Name);

        return this;
    }
}
