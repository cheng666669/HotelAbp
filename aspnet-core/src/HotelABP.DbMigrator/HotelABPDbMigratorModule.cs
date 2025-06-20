using HotelABP.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace HotelABP.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(HotelABPEntityFrameworkCoreModule),
    typeof(HotelABPApplicationContractsModule)
    )]
public class HotelABPDbMigratorModule : AbpModule
{
}
