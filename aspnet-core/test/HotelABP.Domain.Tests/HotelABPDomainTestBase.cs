using Volo.Abp.Modularity;

namespace HotelABP;

/* Inherit from this class for your domain layer tests. */
public abstract class HotelABPDomainTestBase<TStartupModule> : HotelABPTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
