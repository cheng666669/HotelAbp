using Volo.Abp.Modularity;

namespace HotelABP;

[DependsOn(
    typeof(HotelABPApplicationModule),
    typeof(HotelABPDomainTestModule)
)]
public class HotelABPApplicationTestModule : AbpModule
{

}
