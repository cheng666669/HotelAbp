using HotelABP.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace HotelABP.Permissions;

public class HotelABPPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(HotelABPPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(HotelABPPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HotelABPResource>(name);
    }
}
