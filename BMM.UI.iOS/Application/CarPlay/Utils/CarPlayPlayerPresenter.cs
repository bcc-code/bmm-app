using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using BMM.Api.Abstraction;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.Extensions;
using CarPlay;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Utils;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public static class CarPlayPlayerPresenter
{
    private static IMediaPlayer MediaPlayer => Mvx.IoCProvider!.Resolve<IMediaPlayer>();

    public static async Task PlayAndShowPlayer(
        IList<IMediaTrack> tracks,
        IMediaTrack trackToPlay,
        string playbackOrigin,
        CPInterfaceController cpInterfaceController)
    {
        await MediaPlayer.Play(
            tracks,
            trackToPlay,
            playbackOrigin);
        
        var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
        nowPlayingTemplate.UpdateNowPlayingButtons(new[]
        {
            new CPNowPlayingImageButton(
                UIImage.FromBundle(ImageResourceNames.SkipBackIcon.ToStandardIosImageName()),
                button =>
                {
                    MediaPlayer.JumpBackward();
                })
            {
                Enabled = true
            },
            new CPNowPlayingImageButton(
                UIImage.FromBundle(ImageResourceNames.ShuffleIcon.ToStandardIosImageName()),
                button =>
                {
                    MediaPlayer.ToggleShuffle();
                    button.BeginInvokeOnMainThread(() => { button.Selected = !MediaPlayer.IsShuffleEnabled; });
                })
            {
                Enabled = true,
                Selected = MediaPlayer.IsShuffleEnabled
            },
            new CPNowPlayingImageButton(
                UIImage.FromBundle(ImageResourceNames.SkipForwardIcon.ToStandardIosImageName()),
                button =>
                {
                    MediaPlayer.JumpForward();
                })
            {
                Enabled = true,
            }
        });
        
        await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
    }
}