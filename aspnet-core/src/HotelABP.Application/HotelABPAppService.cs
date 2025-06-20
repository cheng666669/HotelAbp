using System;
using System.Collections.Generic;
using System.Text;
using HotelABP.Localization;
using Volo.Abp.Application.Services;

namespace HotelABP;

/* Inherit your application services from this class.
 */
public abstract class HotelABPAppService : ApplicationService
{
    protected HotelABPAppService()
    {
        LocalizationResource = typeof(HotelABPResource);
    }
}
