using BMM.Core.Implementations.Device;
using BMM.UI.iOS.Utils;
using Microsoft.Maui.ApplicationModel;
using AppTheme = BMM.Api.Implementation.Models.Enums.AppTheme;

namespace BMM.UI.iOS.Implementations.Device;

public class iOSDeviceInfo : BaseDeviceInfo
{
    public override async Task<AppTheme> GetCurrentTheme()
    {
        return await MainThread.InvokeOnMainThreadAsync(() => ThemeUtils.IsUsingDarkMode
            ? AppTheme.Dark
            : AppTheme.Light);
    }
}