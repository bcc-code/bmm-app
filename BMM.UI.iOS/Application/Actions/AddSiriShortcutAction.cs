using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Siri.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Utils;
using MvvmCross.Base;

namespace BMM.UI.iOS.Actions
{
    public class AddSiriShortcutAction 
        : GuardedActionWithParameter<StandardSelectablePO>,
          IAddSiriShortcutAction
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

        public AddSiriShortcutAction(
            IUserDialogs userDialogs,
            IBMMLanguageBinder bmmLanguageBinder,
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
        {
            _userDialogs = userDialogs;
            _bmmLanguageBinder = bmmLanguageBinder;
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
        }
        
        protected override async Task Execute(StandardSelectablePO siriShortcut)
        {
            await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                if (siriShortcut.IsSelected)
                    _userDialogs.Toast(new ToastConfig(_bmmLanguageBinder[Translations.SiriShortcutsViewModel_ShortcutAlreadyAdded]));

                switch (siriShortcut.Value)
                {
                    case SiriConstants.FromKareIdentifier:
                    {
                        SiriUtils.AddFromKaareShortcut();
                        break;
                    }
                    case SiriConstants.PlayMusicIdentifier:
                    {
                        SiriUtils.AddPlayMusicShortcut();
                        break;
                    }
                }
            });
        }
    }
}