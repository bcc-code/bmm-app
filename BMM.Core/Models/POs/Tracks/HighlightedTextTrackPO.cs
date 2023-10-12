using System.Text.RegularExpressions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.UI.StyledText;
using BMM.Core.Implementations.UI.StyledText.Enums;
using BMM.Core.Implementations.UI.StyledText.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.Tracks;

public class HighlightedTextTrackPO : DocumentPO, IHighlightedTextTrackPO
{
    private const string RegexToExtractHighlightedParts = @"\*\*\|(\w+)\*\*/";
    
    public HighlightedTextTrackPO(HighlightedTextTrack highlightedTextTrack,
        ITrackPO trackPO,
        IMvxNavigationService mvxNavigationService,
        IMediaPlayer mediaPlayer,
        IShareLink shareLink,
        IAnalytics analytics) : base(highlightedTextTrack)
    {
        HighlightedTextTrack = highlightedTextTrack;
        TrackPO = trackPO;
        ItemClickedCommand = new ExceptionHandlingCommand(() =>
        {
            analytics.LogEvent("Open HighlightedTextTrack",
                new Dictionary<string, object>
                {
                    {"TrackId", highlightedTextTrack.Track.Id},
                    {"TypeOfTrack", highlightedTextTrack.Track.Subtype},
                    {"NumberOfHighlights", highlightedTextTrack.SearchHighlights.Count},
                    {"ItemIndex", highlightedTextTrack.ItemIndex}
                });
            return mvxNavigationService.Navigate<HighlightedTextTrackViewModel, HighlightedTextTrackPO>(this);
        });
        
        foreach (var searchHighlight in highlightedTextTrack.SearchHighlights)
        {
            var styledTexts = ExtractParts(searchHighlight.Text);

            StyledTextContainer ??= new StyledTextContainer(styledTexts,
                13,
                false,
                1f);
            
            HighlightedTexts.Add(new HighlightedTextPO(
                trackPO,
                new StyledTextContainer(styledTexts, 16, true, 1.5f),
                searchHighlight,
                mediaPlayer,
                shareLink,
                mvxNavigationService));
        }

        var firstHighlight = StyledTextContainer
            !.StyledTexts
            .FirstOrDefault(s => s.Style == TextStyle.Highlighted);

        RatioOfFirstHighlightPositionToFullText = firstHighlight == null || firstHighlight.Position == 0
            ? 0
            : firstHighlight.Position / (float)StyledTextContainer.FullText.Length;
        
        RatioOfFirstHighlightLengthToFullText = firstHighlight == null
            ? 0
            : firstHighlight.Length / (float)StyledTextContainer.FullText.Length;
    }
    
    public StyledTextContainer StyledTextContainer { get; }
    public float RatioOfFirstHighlightPositionToFullText { get; }
    public float RatioOfFirstHighlightLengthToFullText { get; set; }
    public HighlightedTextTrack HighlightedTextTrack { get; }
    public ITrackPO TrackPO { get; }
    public IList<IHighlightedTextPO> HighlightedTexts { get; } = new List<IHighlightedTextPO>();
    public IMvxAsyncCommand ItemClickedCommand { get; }
    
    private IList<IStyledText> ExtractParts(string input) {
        
        var parts = new List<IStyledText>();
        
        var pattern = new Regex(RegexToExtractHighlightedParts);
        int startIndex = 0;
        var matches = pattern.Matches(input);
        
        foreach (Match match in matches) 
        {
            string beforeToken = input.Substring(startIndex, match.Index - startIndex);
            
            if (!string.IsNullOrEmpty(beforeToken))
                parts.Add(new StyledText(TextStyle.Default, beforeToken, startIndex));
            
            parts.Add(new StyledText(TextStyle.Highlighted, match.Groups[1].Value, match.Index));
            startIndex = match.Index + match.Length;
        }
        
        string afterTokens = input.Substring(startIndex);
        
        if (!string.IsNullOrEmpty(afterTokens)) 
            parts.Add(new StyledText(TextStyle.Default, afterTokens, startIndex));
        
        return parts;
    }
}