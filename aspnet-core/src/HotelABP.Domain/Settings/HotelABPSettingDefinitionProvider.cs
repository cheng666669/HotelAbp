using Volo.Abp.Settings;

namespace HotelABP.Settings;

public class HotelABPSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(HotelABPSettings.MySetting1));
    }
}
