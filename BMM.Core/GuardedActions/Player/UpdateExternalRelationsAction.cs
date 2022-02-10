using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Api.Utils;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Player
{
    public class UpdateExternalRelationsAction : GuardedAction, IUpdateExternalRelationsAction
    {
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;

        public UpdateExternalRelationsAction(IFirebaseRemoteConfig firebaseRemoteConfig)
        {
            _firebaseRemoteConfig = firebaseRemoteConfig;
        }
        
        private IPlayerViewModel PlayerViewModel => this.GetDataContext();

        protected override Task Execute()
        {
            var currentTrack = PlayerViewModel.CurrentTrack;
            
            if (currentTrack == null)
                return Task.CompletedTask;

            PlayerViewModel.TrackLanguage = new CultureInfo(currentTrack.Language).NativeName;
            PlayerViewModel.HasExternalRelations = currentTrack.Relations != null &&
                                                   currentTrack.Relations.Any(relation => relation.Type == TrackRelationType.External);

            var existingSongbook = currentTrack
                ?.Relations
                ?.OfType<TrackRelationSongbook>()
                .FirstOrDefault();

            string songTreasureLink = existingSongbook != null
                ? string.Format(_firebaseRemoteConfig.SongTreasuresSongLink,
                    SongbookUtils.GetShortName(existingSongbook.Name),
                    existingSongbook.Id)
                : string.Empty;
            
            PlayerViewModel.SongTreasureLink = songTreasureLink;
            return Task.CompletedTask;
        }

        protected override async Task OnFinally()
        {
            await base.OnFinally();
            await PlayerViewModel.RaisePropertyChanged(nameof(PlayerViewModel.HasLyrics));
            PlayerViewModel.OpenLyricsCommand.RaiseCanExecuteChanged();
        }
    }
}
