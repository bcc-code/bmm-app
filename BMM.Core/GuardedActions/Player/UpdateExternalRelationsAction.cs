using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Api.Utils;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Region.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Player
{
    public class UpdateExternalRelationsAction : GuardedAction, IUpdateExternalRelationsAction
    {
        private const string SangtekstRelationName = "Sangtekst";
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
        private readonly ICultureInfoRepository _cultureInfoRepository;

        public UpdateExternalRelationsAction(
            IFirebaseRemoteConfig firebaseRemoteConfig,
            ICultureInfoRepository cultureInfoRepository)
        {
            _firebaseRemoteConfig = firebaseRemoteConfig;
            _cultureInfoRepository = cultureInfoRepository;
        }
        
        private IPlayerViewModel PlayerViewModel => this.GetDataContext();

        protected override Task Execute()
        {
            var currentTrack = PlayerViewModel.CurrentTrack;
            
            if (currentTrack == null)
                return Task.CompletedTask;

            if (currentTrack.Language != ContentLanguageManager.LanguageIndependentContent)
                PlayerViewModel.TrackLanguage = _cultureInfoRepository.Get(currentTrack.Language).NativeName;
            
            PlayerViewModel.HasExternalRelations = currentTrack.Relations != null &&
                                                   currentTrack.Relations.Any(relation => relation.Type == TrackRelationType.External);

            PlayerViewModel.SongTreasureLink =  GetLyricsLink(currentTrack);
            return Task.CompletedTask;
        }

        private string GetLyricsLink(ITrackModel currentTrack)
        {
            var existingSongbook = currentTrack
                ?.Relations
                ?.OfType<TrackRelationSongbook>()
                .FirstOrDefault();

            if (existingSongbook != null)
            {
                return string.Format(_firebaseRemoteConfig.SongTreasuresSongLink,
                    SongbookUtils.GetShortName(existingSongbook.Name),
                    existingSongbook.Id);
            }

            var sangtekstElement = currentTrack
                ?.Relations
                ?.OfType<TrackRelationExternal>()
                .FirstOrDefault(x => string.Equals(x.Name, SangtekstRelationName, StringComparison.OrdinalIgnoreCase));

            if (sangtekstElement == null)
                return string.Empty;

            return sangtekstElement.Url;
        }

        protected override async Task OnFinally()
        {
            await base.OnFinally();
            await PlayerViewModel.RaisePropertyChanged(nameof(PlayerViewModel.HasLyrics));
            PlayerViewModel.OpenLyricsCommand.RaiseCanExecuteChanged();
        }
    }
}
