using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IThemeSettingsViewModel : IBaseViewModel
    {
        IBmmObservableCollection<ThemeSettingPO> ThemeSettings { get; }
        IMvxCommand<ThemeSettingPO> ThemeSelectedCommand { get; }
    }
}