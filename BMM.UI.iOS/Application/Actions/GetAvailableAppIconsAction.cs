using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.Enums;
using BMM.Core.Models.POs;
using MvvmCross.Base;

namespace BMM.UI.iOS.Actions;

public class GetAvailableAppIconsAction : GuardedActionWithResult<IList<AppIconPO>>, IGetAvailableAppIconsAction
{
    private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadDispatcher;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;

    public GetAvailableAppIconsAction(
        IMvxMainThreadAsyncDispatcher mvxMainThreadDispatcher,
        IBMMLanguageBinder bmmLanguageBinder)
    {
        _mvxMainThreadDispatcher = mvxMainThreadDispatcher;
        _bmmLanguageBinder = bmmLanguageBinder;
    }
    
    protected override async Task<IList<AppIconPO>> Execute()
    {
        var listOfIcons = new List<AppIconPO>();

        await _mvxMainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
        {
            string currentIconName = UIApplication.SharedApplication.AlternateIconName;

            foreach (AppIconType enumValue in typeof(AppIconType).GetEnumValues())
            {
                var appIcon = new AppIconPO(enumValue, _bmmLanguageBinder[$"AppIconViewModel_{enumValue}"], $"App{enumValue}");
                listOfIcons.Add(appIcon);

                if (enumValue == AppIconType.IconStandard && currentIconName == null)
                    appIcon.IsSelected = true;
                else
                    appIcon.IsSelected = currentIconName == enumValue.ToString();
            }
        });

        return listOfIcons;
    }
}