using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces;

public interface IAppIconViewModel : IBaseViewModel
{
    IBmmObservableCollection<AppIconPO> AppIcons { get; }
    IMvxAsyncCommand<AppIconPO> AppIconSelected { get; }
    void SelectTheme(AppIconPO appIcon);
}