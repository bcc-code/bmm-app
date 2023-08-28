using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Implementations.Device;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.Device;

public class DroidDeviceInfo : BaseDeviceInfo
{
    private readonly IMvxAndroidCurrentTopActivity _currentTopActivity;

    public DroidDeviceInfo(IMvxAndroidCurrentTopActivity currentTopActivity)
    {
        _currentTopActivity = currentTopActivity;
    }

    public override Task<AppTheme> GetCurrentTheme() =>
        Task.FromResult(_currentTopActivity.Activity.IsNightMode()
            ? AppTheme.Dark
            : AppTheme.Light);
}