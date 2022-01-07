using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.POs;
using BMM.Core.Models.Themes;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class ThemeSettingsViewModel : BaseViewModel, IThemeSettingsViewModel
    {
        public ThemeSettingsViewModel(IChangeThemeSettingsAction changeThemeSettingsAction)
        {
            ThemeSelectedCommand = new MvxCommand<ThemeSettingPO>(async s =>
            {
                await changeThemeSettingsAction.ExecuteGuarded(s.Theme);
                AppSettings.SelectedTheme = s.Theme;
                SelectTheme(s.Theme);
            });
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var settingsList = new List<ThemeSettingPO>
            {
                new(Theme.Light, TextSource[GetType().GetTranslationKey(Theme.Light.ToString())]),
                new(Theme.Dark, TextSource[GetType().GetTranslationKey(Theme.Dark.ToString())]),
                new(Theme.System, TextSource[GetType().GetTranslationKey(Theme.System.ToString())])
            };

            var appThemeSetting = AppSettings.SelectedTheme;
            ThemeSettings.AddRange(settingsList);
            SelectTheme(appThemeSetting);
        }

        private void SelectTheme(Theme theme)
        {
            ThemeSettings
                .FirstOrDefault(s => s.IsSelected)
                .IfNotNull(s => s.IsSelected = false);

            ThemeSettings
                .FirstOrDefault(s => s.Theme == theme)
                .IfNotNull(s => s.IsSelected = true);
        }

        public IBmmObservableCollection<ThemeSettingPO> ThemeSettings { get; }
            = new BmmObservableCollection<ThemeSettingPO>();

        public IMvxCommand<ThemeSettingPO> ThemeSelectedCommand { get; }
    }
}