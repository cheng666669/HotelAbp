using Xunit;

namespace HotelABP.EntityFrameworkCore;

[CollectionDefinition(HotelABPTestConsts.CollectionDefinitionName)]
public class HotelABPEntityFrameworkCoreCollection : ICollectionFixture<HotelABPEntityFrameworkCoreFixture>
{

}
