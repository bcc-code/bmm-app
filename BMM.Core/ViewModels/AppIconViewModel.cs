using BMM.Core.Extensions;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels;

public class AppIconViewModel : BaseViewModel, IAppIconViewModel
{
    private readonly IGetAvailableAppIconsAction _getAvailableAppIconsAction;

    public AppIconViewModel(
        IGetAvailableAppIconsAction getAvailableAppIconsAction,
        IAppIconSelectedAction appIconSelectedAction)
    {
        _getAvailableAppIconsAction = getAvailableAppIconsAction;
        appIconSelectedAction.AttachDataContext(this);
        AppIconSelected = new ExceptionHandlingCommand<AppIconPO>(async (appIconPO) =>
        {
            SelectTheme(appIconPO);
            await appIconSelectedAction.ExecuteGuarded(appIconPO.AppIconType);
        });
    }

    public override async Task Initialize()
    {
        await base.Initialize();
        var icons = await _getAvailableAppIconsAction.ExecuteGuarded();
        AppIcons.AddRange(icons);
    }

    public IBmmObservableCollection<AppIconPO> AppIcons { get; } = new BmmObservableCollection<AppIconPO>();
    public IMvxAsyncCommand<AppIconPO> AppIconSelected { get; }
    
    public void SelectTheme(AppIconPO appIcon)
    {
        AppIcons
            .FirstOrDefault(s => s.IsSelected)
            .IfNotNull(s => s.IsSelected = false);

        AppIcons
            .FirstOrDefault(s => s.AppIconType == appIcon.AppIconType)
            .IfNotNull(s => s.IsSelected = true);
    }
}