using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.SettingManagement;

namespace HotelABP;

[DependsOn(
    typeof(HotelABPDomainSharedModule),

    typeof(AbpSettingManagementApplicationContractsModule),

    typeof(AbpObjectExtendingModule)
)]
public class HotelABPApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        HotelABPDtoExtensions.Configure();
    }
}
