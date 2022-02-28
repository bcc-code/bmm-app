using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Tracks.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.Translation;
using BMM.UI.iOS.Extensions;
using MvvmCross.Base;

namespace BMM.UI.iOS.Implementations.Track
{
    public class iOSTrackOptionsService : ITrackOptionsService
    {
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IUserDialogs _userDialogs;
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

        public iOSTrackOptionsService(IBMMLanguageBinder bmmLanguageBinder, IUserDialogs userDialogs, IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
        {
            _bmmLanguageBinder = bmmLanguageBinder;
            _userDialogs = userDialogs;
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
        }

        public async Task OpenOptions(IList<StandardIconOptionPO> optionsList)
        {
            var actionSheet = new ActionSheetConfig();
            foreach (var standardIconOptionPO in optionsList)
            {
                actionSheet.AddHandled(
                    standardIconOptionPO.Title,
                    () => standardIconOptionPO.ClickCommand.ExecuteAsync(),
                    standardIconOptionPO.ImagePath?.ToStandardIosImageName());
            }
            
            actionSheet.SetCancel(_bmmLanguageBinder[Translations.UserDialogs_Cancel]);
            await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() => _userDialogs.ActionSheet(actionSheet));
        }
    }
}