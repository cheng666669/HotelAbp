using Microsoft.Extensions.Localization;
using HotelABP.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace HotelABP;

[Dependency(ReplaceServices = true)]
public class HotelABPBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<HotelABPResource> _localizer;

    public HotelABPBrandingProvider(IStringLocalizer<HotelABPResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
