using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly IViewModelAwareViewPresenter _viewModelAwareViewPresenter;
        private readonly IAnalytics _analytics;

        public MenuViewModel(
            IViewModelAwareViewPresenter viewModelAwareViewPresenter,
            IAnalytics analytics)
        {
            _viewModelAwareViewPresenter = viewModelAwareViewPresenter;
            _analytics = analytics;
        }

        public IDictionary<string, IMvxCommand> NavigationCommands { get; } = new Dictionary<string, IMvxCommand>();

        public override Task Initialize()
        {
            NavigationCommands.Clear();
            NavigationCommands.Add(nameof(ExploreNewestViewModel), MenuEntry<ExploreNewestViewModel>());
            NavigationCommands.Add(nameof(BrowseViewModel), MenuEntry<BrowseViewModel>());
            NavigationCommands.Add(nameof(SearchViewModel), MenuEntry<SearchViewModel>());
            NavigationCommands.Add(nameof(MyContentViewModel), MenuEntry<MyContentViewModel>());
            NavigationCommands.Add(nameof(SettingsViewModel), MenuEntry<SettingsViewModel>());

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

        public void LogBottomBarButtonClicked(string viewModelName)
        {
            _analytics.LogEvent(Event.BottomBarButtonClicked, new Dictionary<string, object>()
            {
                {"ViewModel", viewModelName}
            });
        }
    }
}