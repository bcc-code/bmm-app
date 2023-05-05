using BMM.Core.GuardedActions.HighlightedText.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tracks.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tracks;

public class HighlightedTextHeaderPO : DocumentPO, IHighlightedTextHeaderPO
{
    private readonly IShowHighlightedTextInfoDialogAction _showHighlightedTextInfoDialogAction;

    public HighlightedTextHeaderPO(IShowHighlightedTextInfoDialogAction showHighlightedTextInfoDialogAction) : base(null)
    {
        _showHighlightedTextInfoDialogAction = showHighlightedTextInfoDialogAction;
    }

    public IMvxAsyncCommand ItemClickedCommand => _showHighlightedTextInfoDialogAction.Command;
}