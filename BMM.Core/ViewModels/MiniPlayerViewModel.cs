using BMM.Core.Helpers;
using BMM.Core.Messages;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;
using Xamarin.Essentials;

namespace BMM.Core.ViewModels
{
    public sealed class MiniPlayerViewModel : PlayerBaseViewModel
    {
        public MiniPlayerViewModel(IMediaPlayer mediaPlayer)
            : base(mediaPlayer)
        {
            OpenPlayerCommand = new ExceptionHandlingCommand(async () =>
            {
                // since Android does not use proper ViewModel navigation to show the PlayerViewModel we have to use this bit hacky solution.
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    _messenger.Publish(new TogglePlayerMessage(this, true));
                else
                    await _navigationService.Navigate<PlayerViewModel>();
            });
            UpdatePlaybackState(MediaPlayer.PlaybackState);
        }

        public IMvxCommand OpenPlayerCommand { get; set; }
    }
}