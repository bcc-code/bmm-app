using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.Player
{
    public class ChangeTrackLanguageAction : GuardedAction, IChangeTrackLanguageAction
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly ITracksClient _tracksClient;

        public ChangeTrackLanguageAction(
            IMediaPlayer mediaPlayer,
            IMvxNavigationService mvxNavigationService,
            ITracksClient tracksClient)
        {
            _mediaPlayer = mediaPlayer;
            _mvxNavigationService = mvxNavigationService;
            _tracksClient = tracksClient;
        }
        
        private IPlayerViewModel PlayerViewModel => this.GetDataContext();
        
        protected override async Task Execute()
        {
            string selectedLang = await _mvxNavigationService.Navigate<ChangeTrackLanguageViewModel, ITrackModel, string>(PlayerViewModel.CurrentTrack);
            
            if (string.IsNullOrEmpty(selectedLang) || selectedLang == PlayerViewModel.CurrentTrack?.Language)
                return;

            var track = await _tracksClient.GetById(_mediaPlayer.CurrentTrack.Id, selectedLang);
            await _mediaPlayer.ReplaceTrack(track);
        }

        protected override bool CanExecute()
        {
            var currentTrack = PlayerViewModel.CurrentTrack;
            
            bool hasLanguagesToChange = currentTrack?.AvailableLanguages != null
                                        && currentTrack.AvailableLanguages.Count() > 1;
            
            return base.CanExecute() && hasLanguagesToChange;
        }
    }
}