using BMM.Core.Helpers;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Messages;
using BMM.Core.NewMediaPlayer.Abstractions;
using Microsoft.Maui.Devices;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public sealed class MiniPlayerViewModel : PlayerBaseViewModel
    {
        private MiniPlayerTrackInfoProvider _miniPlayerTrackInfoProvider;

        public MiniPlayerViewModel(IMediaPlayer mediaPlayer)
            : base(mediaPlayer)
        {
            OpenPlayerCommand = new ExceptionHandlingCommand(async () =>
            {
                // since Android does not use proper ViewModel navigation to show the PlayerViewModel we have to use this bit hacky solution.
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    Messenger.Publish(new TogglePlayerMessage(this, true));
                else
                    await NavigationService.Navigate<PlayerViewModel>();
            });
            UpdatePlaybackState(MediaPlayer.PlaybackState);
        }

        public IMvxCommand OpenPlayerCommand { get; set; }
        public override ITrackInfoProvider TrackInfoProvider => _miniPlayerTrackInfoProvider ??= new MiniPlayerTrackInfoProvider();
    }
}