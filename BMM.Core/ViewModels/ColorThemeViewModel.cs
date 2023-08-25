using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.POs;
using BMM.Core.Models.Themes;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class ColorThemeViewModel : BaseViewModel
    {
        public ColorThemeViewModel(IChangeColorThemeAction changeColorThemeAction)
        {
            ColorThemeSelectedCommand = new MvxCommand<ColorThemePO>(async s =>
            {
                await changeColorThemeAction.ExecuteGuarded(s.ColorTheme);
                SelectTheme(s.ColorTheme);
            });
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var colorThemesList = new List<ColorThemePO>
            {
                new(ColorTheme.Default, TextSource[GetType().GetTranslationKey(ColorTheme.Default.ToString())]),
            };
            
            colorThemesList.AddIf(() => AppSettings.DarkGreenRewardUnlocked, new(ColorTheme.DarkGreen, TextSource[GetType().GetTranslationKey(ColorTheme.DarkGreen.ToString())]));
            colorThemesList.AddIf(() => AppSettings.OrangeRewardUnlocked, new(ColorTheme.Orange, TextSource[GetType().GetTranslationKey(ColorTheme.Orange.ToString())]));
            colorThemesList.AddIf(() => AppSettings.VioletRewardUnlocked, new(ColorTheme.Violet, TextSource[GetType().GetTranslationKey(ColorTheme.Violet.ToString())]));
            colorThemesList.AddIf(() => AppSettings.RedRewardUnlocked, new(ColorTheme.Red, TextSource[GetType().GetTranslationKey(ColorTheme.Red.ToString())]));
            colorThemesList.AddIf(() => AppSettings.GoldenRewardUnlocked, new(ColorTheme.Golden, TextSource[GetType().GetTranslationKey(ColorTheme.Golden.ToString())]));
            
            var appThemeSetting = AppSettings.SelectedColorTheme;
            ColorThemes.AddRange(colorThemesList);
            SelectTheme(appThemeSetting);
        }

        private void SelectTheme(ColorTheme colorTheme)
        {
            ColorThemes
                .FirstOrDefault(s => s.IsSelected)
                .IfNotNull(s => s.IsSelected = false);

            ColorThemes
                .FirstOrDefault(s => s.ColorTheme == colorTheme)
                .IfNotNull(s => s.IsSelected = true);
        }

        public IBmmObservableCollection<ColorThemePO> ColorThemes { get; }
            = new BmmObservableCollection<ColorThemePO>();

        public IMvxCommand<ColorThemePO> ColorThemeSelectedCommand { get; }
    }
}