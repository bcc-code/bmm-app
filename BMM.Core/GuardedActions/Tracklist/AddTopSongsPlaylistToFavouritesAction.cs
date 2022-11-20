using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.Tracklist
{
    public class AddTopSongsPlaylistToFavouritesAction
        : GuardedAction,
          IAddTopSongsPlaylistToFavouritesAction
    {
        private readonly ITrackCollectionClient _trackCollectionClient;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IBMMLanguageBinder _languageBinder;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly IAnalytics _analytics;

        public AddTopSongsPlaylistToFavouritesAction(
            ITrackCollectionClient trackCollectionClient,
            IToastDisplayer toastDisplayer,
            IBMMLanguageBinder languageBinder,
            IMvxNavigationService mvxNavigationService,
            IAnalytics analytics)
        {
            _trackCollectionClient = trackCollectionClient;
            _toastDisplayer = toastDisplayer;
            _languageBinder = languageBinder;
            _mvxNavigationService = mvxNavigationService;
            _analytics = analytics;
        }
        
        private ITopSongsCollectionViewModel DataContext => this.GetDataContext();
        
        protected override async Task Execute()
        {
            await _trackCollectionClient.AddTopSongsToFavourites();
            await DataContext.CloseCommand.ExecuteAsync();
            await _toastDisplayer.Success(_languageBinder[Translations.TopSongsCollectionViewModel_SuccessfullyAddedToPlaylists]);
            await _mvxNavigationService.Navigate<MyContentViewModel>();
            _analytics.LogEvent(Event.YearInReviewPlaylistAddedToFavorites);
        }
    }
}