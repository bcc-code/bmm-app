using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Actions.Interfaces;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS.Actions
{
    public class SearchForMusicFromSiriAction : 
        GuardedActionWithParameterAndResult<string, bool>,
        ISearchForMusicFromSiriAction
    {
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IPodcastClient _podcastClient;
        private readonly IMediaPlayer _mediaPlayer;
        private string FromKareCurrentLanguageTranslation => _bmmLanguageBinder[Translations.ExploreNewestViewModel_FraKaareHeader];

        public SearchForMusicFromSiriAction(
            IBMMLanguageBinder bmmLanguageBinder,
            IPodcastClient podcastClient,
            IMediaPlayer mediaPlayer)
        {
            _bmmLanguageBinder = bmmLanguageBinder;
            _podcastClient = podcastClient;
            _mediaPlayer = mediaPlayer;
        }
        
        protected override async Task<bool> Execute(string searchPhrase)
        {
            if (SiriConstants.FromKareSiriPhrases.Contains(searchPhrase) || searchPhrase == FromKareCurrentLanguageTranslation)
                return await PlayFromKare();

            return false;
        }

        private async Task<bool> PlayFromKare()
        {
            var fromKareList = await _podcastClient.GetTracks(FraKaareTeaserViewModel.FraKårePodcastId, CachePolicy.IgnoreCache);
            
            if (fromKareList == null || !fromKareList.Any())
                return false;

            var fromKareMediaTracks = fromKareList
                .OfType<IMediaTrack>()
                .ToList();

            await _mediaPlayer.Play(fromKareMediaTracks, fromKareMediaTracks.First(), SiriConstants.PlaybackOrigin);
            return true;
        }
    }
}