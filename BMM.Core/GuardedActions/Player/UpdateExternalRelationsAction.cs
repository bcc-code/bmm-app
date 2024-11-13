using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Api.Utils;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Implementations;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Region.Interfaces;
using BMM.Core.Models.Enums;
using BMM.Core.Models.Player.Lyrics;
using BMM.Core.Utils;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Player
{
    public class UpdateExternalRelationsAction : GuardedAction, IUpdateExternalRelationsAction
    {
        private readonly IReadOnlyList<string> _bccMediaDomains = new[]
        {
            "app.bcc.media",
            "brunstad.tv"
        };
        
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
        private readonly ICultureInfoRepository _cultureInfoRepository;
        private readonly IDeveloperPermission _developerPermission;
        private readonly IGetLyricsLinkAction _getLyricsLinkAction;

        public UpdateExternalRelationsAction(
            IFirebaseRemoteConfig firebaseRemoteConfig,
            ICultureInfoRepository cultureInfoRepository,
            IDeveloperPermission developerPermission,
            IGetLyricsLinkAction getLyricsLinkAction)
        {
            _firebaseRemoteConfig = firebaseRemoteConfig;
            _cultureInfoRepository = cultureInfoRepository;
            _developerPermission = developerPermission;
            _getLyricsLinkAction = getLyricsLinkAction;
        }
        
        private IPlayerViewModel PlayerViewModel => this.GetDataContext();

        protected override async Task Execute()
        {
            var currentTrack = PlayerViewModel.CurrentTrack;

            if (currentTrack == null)
                return;

            if (currentTrack.Language != ContentLanguageManager.LanguageIndependentContent)
                PlayerViewModel.TrackLanguage = _cultureInfoRepository.GetCultureInfoLanguage(currentTrack.Language).NativeName;
            
            PlayerViewModel.HasExternalRelations = currentTrack.Relations != null &&
                                                   currentTrack.Relations.Any(relation => relation.Type == TrackRelationType.External);

            PlayerViewModel.WatchBccMediaLink = GetBCCMediaLink(currentTrack);
            var lyricsLink = await _getLyricsLinkAction.ExecuteGuarded(currentTrack);

            PlayerLeftButtonType leftButtonType = default;

            bool isReadingTranscriptionsEnabled =
                _firebaseRemoteConfig.IsReadingTranscriptionsEnabled || _developerPermission.IsBmmDeveloper();
            
            bool hasTranscription = currentTrack.HasTranscription && isReadingTranscriptionsEnabled;
            
            if (hasTranscription)
            {
                if (currentTrack.IsSong())
                    leftButtonType = PlayerLeftButtonType.Lyrics;
                else
                    leftButtonType = PlayerLeftButtonType.Transcription;
            }
            else if (lyricsLink.LyricsLinkType != LyricsLinkType.None)
                leftButtonType = PlayerLeftButtonType.Lyrics;
            
            PlayerViewModel.LeftButtonType = leftButtonType;
            PlayerViewModel.LeftButtonLink = lyricsLink.Link;
            PlayerViewModel.HasTranscription = hasTranscription;
        }

        private string GetBCCMediaLink(ITrackModel currentTrack)
        {
            var externalRelation = currentTrack
                ?.Relations
                ?.OfType<TrackRelationExternal>()
                .FirstOrDefault();

            if (string.IsNullOrEmpty(externalRelation?.Url) || !UriUtils.TryCreate(externalRelation.Url, out var uri))
                return null;

            if (_bccMediaDomains.Contains(uri.Host))
                return externalRelation.Url;

            return null;
        }

        protected override async Task OnFinally()
        {
            await base.OnFinally();
            await PlayerViewModel.RaisePropertyChanged(nameof(PlayerViewModel.HasLeftButton));
            await PlayerViewModel.RaisePropertyChanged(nameof(PlayerViewModel.HasWatchButton));
            PlayerViewModel.LeftButtonClickedCommand.RaiseCanExecuteChanged();
        }
    }
}
