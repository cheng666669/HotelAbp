using HotelABP.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace HotelABP.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class HotelABPController : AbpControllerBase
{
    protected HotelABPController()
    {
        LocalizationResource = typeof(HotelABPResource);
    }
}
