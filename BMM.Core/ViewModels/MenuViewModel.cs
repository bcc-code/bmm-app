using System;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Notifications;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using BMM.Core.ViewModels.Parameters;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public IMvxCommand SearchCommand { get; private set; }

        public IMvxCommand ExploreCommand { get; private set; }

        public IMvxCommand MyContentCommand { get; private set; }

        public IMvxCommand LibraryCommand { get; private set; }

        public IMvxCommand SettingsCommand { get; private set; }

        public override Task Initialize()
        {
            SearchCommand = MenuEntry<SearchViewModel>();
            ExploreCommand = MenuEntry<ExploreNewestViewModel>();
            MyContentCommand = MenuEntry<MyContentViewModel>();
            LibraryCommand = MenuEntry<LibraryViewModel>();
            SettingsCommand = MenuEntry<SettingsViewModel>();

            return base.Initialize();
        }

        public IMvxCommand MenuEntry<T>(Func<bool> canExecute = null) where T : IMvxViewModel
        {
            return new ExceptionHandlingCommand(async () =>
                {
                    await _navigationService.ChangePresentation(new MenuClickedHint());
                    await _navigationService.NavigateToNewRoot<T>();
                },
                canExecute
            );
        }
    }
}