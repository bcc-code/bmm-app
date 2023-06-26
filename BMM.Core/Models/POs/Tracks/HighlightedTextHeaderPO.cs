using BMM.Core.GuardedActions.HighlightedText.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.Translation;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tracks;

public class HighlightedTextHeaderPO : DocumentPO, IHighlightedTextHeaderPO
{
    private readonly IShowHighlightedTextInfoDialogAction _showHighlightedTextInfoDialogAction;
    private readonly bool _isSong;

    public HighlightedTextHeaderPO(
        IShowHighlightedTextInfoDialogAction showHighlightedTextInfoDialogAction,
        bool isSong) : base(null)
    {
        _showHighlightedTextInfoDialogAction = showHighlightedTextInfoDialogAction;
        _isSong = isSong;
    }

    public string HeaderText =>
        _isSong
            ? TextSource[Translations.HighlightedTextTrackViewModel_FromSongTreasures]
            : TextSource[Translations.HighlightedTextTrackViewModel_AutoTranscribed];
    
    public IMvxAsyncCommand ItemClickedCommand => _showHighlightedTextInfoDialogAction.Command;
}