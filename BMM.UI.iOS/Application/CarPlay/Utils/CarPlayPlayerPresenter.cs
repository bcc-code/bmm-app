using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.Models.TrackCollections;
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
    private static ILikeUnlikeTrackAction LikeUnlikeTrackAction => Mvx.IoCProvider!.Resolve<ILikeUnlikeTrackAction>();

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

        await ShowPlayer(trackToPlay, cpInterfaceController);
    }
    
    public static async Task ShowPlayer(
        IMediaTrack trackToPlay,
        CPInterfaceController cpInterfaceController)
    {
        var nowPlayingTemplate = CreateButtons(trackToPlay);
        await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
    }

    private static CPNowPlayingTemplate CreateButtons(IMediaTrack track)
    {
        var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
        nowPlayingTemplate.UpdateNowPlayingButtons([
            new CPNowPlayingImageButton(
                UIImage.FromBundle(ImageResourceNames.SkipBackIcon.ToStandardIosImageName())!,
                _ =>
                {
                    MediaPlayer.JumpBackward();
                })
            {
                Enabled = true
            },
            new CPNowPlayingImageButton(
                UIImage.FromBundle(ImageResourceNames.SkipForwardIcon.ToStandardIosImageName())!,
                _ =>
                {
                    MediaPlayer.JumpForward();
                })
            {
                Enabled = true,
            },
            new CPNowPlayingImageButton(
                UIImage.FromBundle(ImageResourceNames.ShuffleIcon.ToStandardIosImageName())!,
                ShuffleButtonHandler)
            {
                Enabled = true,
                Selected = MediaPlayer.IsShuffleEnabled
            },
            new CPNowPlayingImageButton(
                track != null && track.IsLiked
                ? UIImage.FromBundle(ImageResourceNames.IconLiked.ToStandardIosImageName())!
                : UIImage.FromBundle(ImageResourceNames.IconUnliked.ToStandardIosImageName())!,
                LikeButtonHandler)
            {
                Enabled = true
            },
        ]);
        return nowPlayingTemplate;
    }

    private static void ShuffleButtonHandler(CPNowPlayingButton button)
    {
        MediaPlayer.ToggleShuffle();
        CreateButtons(MediaPlayer.CurrentTrack as IMediaTrack);
    }
    
    private static async void LikeButtonHandler(CPNowPlayingButton button)
    {
        await LikeUnlikeTrackAction.ExecuteGuarded(new LikeOrUnlikeTrackActionParameter(MediaPlayer.CurrentTrack.IsLiked, MediaPlayer.CurrentTrack.Id));
        CreateButtons(MediaPlayer.CurrentTrack as IMediaTrack);
    }
}