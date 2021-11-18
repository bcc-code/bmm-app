using System;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Commands;
using MvvmCross.Presenters;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly IViewModelAwareViewPresenter _viewModelAwareViewPresenter;

        public MenuViewModel(IViewModelAwareViewPresenter viewModelAwareViewPresenter)
        {
            _viewModelAwareViewPresenter = viewModelAwareViewPresenter;
        }

        public IMvxCommand SearchCommand { get; private set; }

        public IMvxCommand ExploreCommand { get; private set; }

        public IMvxCommand MyContentCommand { get; private set; }

        public IMvxCommand BrowseCommand { get; private set; }

        public IMvxCommand SettingsCommand { get; private set; }

        public override Task Initialize()
        {
            SearchCommand = MenuEntry<SearchViewModel>();
            ExploreCommand = MenuEntry<ExploreNewestViewModel>();
            MyContentCommand = MenuEntry<MyContentViewModel>();
            BrowseCommand = MenuEntry<BrowseViewModel>();
            SettingsCommand = MenuEntry<SettingsViewModel>();

            return base.Initialize();
        }

        public IMvxCommand MenuEntry<T>(Func<bool> canExecute = null) where T : IMvxViewModel
        {
            return new ExceptionHandlingCommand(async () =>
                {
                    if (_viewModelAwareViewPresenter.IsViewModelShown<T>())
                        return;

                    await NavigationService.ChangePresentation(new MenuClickedHint());
                    await NavigationService.NavigateToNewRoot<T>();
                },
                canExecute
            );
        }
    }
}