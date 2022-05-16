using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Siri.Interfaces;
using BMM.Core.GuardedActions.SiriShortcuts.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class SiriShortcutsViewModel
        : BaseViewModel,
          ISiriShortcutsViewModel
    {
        private readonly IInitializeSiriShortcutsSettingsAction _initializeSiriShortcutsSettingsAction;
        private readonly IAddSiriShortcutAction _addSiriShortcutAction;

        public SiriShortcutsViewModel(
            IInitializeSiriShortcutsSettingsAction initializeSiriShortcutsSettingsAction,
            IAddSiriShortcutAction addSiriShortcutAction)
        {
            _initializeSiriShortcutsSettingsAction = initializeSiriShortcutsSettingsAction;
            _addSiriShortcutAction = addSiriShortcutAction;
            _initializeSiriShortcutsSettingsAction.AttachDataContext(this);
        }

        public IBmmObservableCollection<StandardSelectablePO> AvailableShortcuts { get; } =
            new BmmObservableCollection<StandardSelectablePO>();

        public IMvxCommand<StandardSelectablePO> ShortcutSelectedCommand => _addSiriShortcutAction.Command;

        public override async Task Initialize()
        {
            await _initializeSiriShortcutsSettingsAction.ExecuteGuarded();
        }
    }
}