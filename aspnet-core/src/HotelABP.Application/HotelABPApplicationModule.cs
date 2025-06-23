using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace HotelABP;

[DependsOn(
    typeof(HotelABPDomainModule),
    typeof(HotelABPApplicationContractsModule),

    typeof(AbpSettingManagementApplicationModule)
    )]
public class HotelABPApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<HotelABPApplicationModule>();
        });
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "MyApp1";
        });
    }
}
