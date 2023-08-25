using BMM.Core.Extensions;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Models.Enums;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Base;

namespace BMM.UI.iOS.Actions;

public class AppIconSelectedAction : GuardedActionWithParameter<AppIconType>, IAppIconSelectedAction
{
    private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

    public AppIconSelectedAction(IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
    {
        _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
    }

    protected override Task Execute(AppIconType parameter)
    {
        string icon = null;

        if (parameter != AppIconType.IconStandard)
            icon = parameter.ToString();

        _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
        {
            UIApplication.SharedApplication.SetAlternateIconName(icon, _ => { });
        });
        
        return Task.CompletedTask;
    }
}