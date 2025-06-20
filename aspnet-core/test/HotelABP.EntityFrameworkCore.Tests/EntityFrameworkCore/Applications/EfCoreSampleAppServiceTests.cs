using HotelABP.Samples;
using Xunit;

namespace HotelABP.EntityFrameworkCore.Applications;

[Collection(HotelABPTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<HotelABPEntityFrameworkCoreTestModule>
{

}
