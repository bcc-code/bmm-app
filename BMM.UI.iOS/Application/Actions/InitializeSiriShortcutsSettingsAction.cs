using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.SiriShortcuts.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;
using BMM.UI.iOS.Constants;
using Intents;

namespace BMM.UI.iOS.Actions
{
    public class InitializeSiriShortcutsSettingsAction 
        : GuardedAction,
          IInitializeSiriShortcutsSettingsAction
    {
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public InitializeSiriShortcutsSettingsAction(IBMMLanguageBinder bmmLanguageBinder)
        {
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        private ISiriShortcutsViewModel SiriShortcutsViewModel => this.GetDataContext();
        
        protected override async Task Execute()
        {
            var shortcuts = await INVoiceShortcutCenter.SharedCenter.GetAllVoiceShortcutsAsync();

            var shortcutsOptionsList = new List<StandardSelectablePO>()
            {
                new StandardSelectablePO(
                    _bmmLanguageBinder[Translations.Global_FromKaareSiriShortcutName],
                    SiriConstants.FromKareIdentifier,
                    IsShortcutAlreadyAdded(shortcuts, SiriConstants.FromKareIdentifier)),
                new StandardSelectablePO(
                    _bmmLanguageBinder[Translations.Global_PlayMusicSiriShortcutName],
                    SiriConstants.PlayMusicIdentifier,
                    IsShortcutAlreadyAdded(shortcuts, SiriConstants.PlayMusicIdentifier))
            };
            
            SiriShortcutsViewModel.AvailableShortcuts.ReplaceWith(shortcutsOptionsList);
        }

        private static bool IsShortcutAlreadyAdded(IEnumerable<INVoiceShortcut> shortcuts, string identifier)
        {
            return shortcuts.Any(s =>
                s.Shortcut.Intent is INPlayMediaIntent playMediaIntent
                && playMediaIntent.MediaItems!.First().Identifier == identifier);
        }
    }
}