using Volo.Abp.Modularity;

namespace HotelABP;

public abstract class HotelABPApplicationTestBase<TStartupModule> : HotelABPTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
