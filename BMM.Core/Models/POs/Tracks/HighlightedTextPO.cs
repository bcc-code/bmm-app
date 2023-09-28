using System.Text;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.UI.StyledText;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.Tracks;

public class HighlightedTextPO : DocumentPO, IHighlightedTextPO
{
    public HighlightedTextPO(ITrackPO trackPO,
        StyledTextContainer styledTextContainer,
        SearchHighlight searchHighlight,
        IMediaPlayer mediaPlayer,
        IShareLink shareLink,
        IMvxNavigationService mvxNavigationService): base(null)
    {
        TrackPO = trackPO;
        StyledTextContainer = styledTextContainer;
        SearchHighlight = searchHighlight;
        Text = searchHighlight.Text;
        StartPositionInMs = searchHighlight.StartPositionInSeconds.ToMilliseconds();
        IsSong = trackPO
            .Track
            .Subtype
            .IsOneOf(TrackSubType.Song, TrackSubType.Singsong);
        
        ItemClickedCommand = new ExceptionHandlingCommand(() =>
        {
            return mediaPlayer.Play(
                trackPO.Track.EncloseInArray(),
                trackPO.Track,
                PlaybackOrigins.SearchHighlightedText,
                StartPositionInMs);
        });
        
        ShareCommand = new ExceptionHandlingCommand(async () =>
        {
            var link = shareLink.GetFor(TrackPO.Track, StartPositionInMs);
            var textBuilder = new StringBuilder(StyledTextContainer.FullText);
            textBuilder.AppendLine();
            textBuilder.Append(link);
            
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = textBuilder.ToString()
            });
        });
        
        SuggestEditClickedCommand = new ExceptionHandlingCommand(() =>
        {
            return mvxNavigationService.Navigate<SuggestEditViewModel, HighlightedTextPO>(this);
        });
    }

    public ITrackPO TrackPO { get; }
    public bool IsSong { get; }
    public StyledTextContainer StyledTextContainer { get; }
    public SearchHighlight SearchHighlight { get; }
    public string Text { get; }
    public long StartPositionInMs { get; }
    public IMvxAsyncCommand ItemClickedCommand { get; }
    public IMvxAsyncCommand ShareCommand { get; }
    public IMvxAsyncCommand SuggestEditClickedCommand { get; }
}