using Volo.Abp.Modularity;

namespace HotelABP;

[DependsOn(
    typeof(HotelABPDomainModule),
    typeof(HotelABPTestBaseModule)
)]
public class HotelABPDomainTestModule : AbpModule
{

}
