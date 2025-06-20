using HotelABP.Samples;
using Xunit;

namespace HotelABP.EntityFrameworkCore.Domains;

[Collection(HotelABPTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<HotelABPEntityFrameworkCoreTestModule>
{

}
