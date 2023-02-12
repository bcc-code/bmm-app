using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
using BMM.Core.Models.Enums;
using BMM.Core.Translation;
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

            
            string bccLink = GetBCCMediaLink(currentTrack);

            var leftButtonType = string.IsNullOrEmpty(bccLink)
                ? PlayerLeftButtonType.Lyrics
                : PlayerLeftButtonType.BCCMedia;
            
            PlayerViewModel.LeftButtonType = leftButtonType;
            PlayerViewModel.LeftButtonLink = leftButtonType == PlayerLeftButtonType.Lyrics
                ? GetLyricsLink(currentTrack)
                : bccLink;
            
            return Task.CompletedTask;
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
            await PlayerViewModel.RaisePropertyChanged(nameof(PlayerViewModel.HasLeftButton));
            PlayerViewModel.LeftButtonClickedCommand.RaiseCanExecuteChanged();
        }
    }
}
