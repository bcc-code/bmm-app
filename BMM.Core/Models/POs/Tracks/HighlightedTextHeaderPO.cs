using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.Parameters;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.Tracks;

public class HighlightedTextHeaderPO : DocumentPO, IHighlightedTextHeaderPO
{
    public HighlightedTextHeaderPO(IDialogService dialogService) : base(null)
    {
        ItemClickedCommand = new ExceptionHandlingCommand(() =>
        {
            return dialogService.ShowDialog(new DialogParameter(
                TextSource[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupTitleText],
                TextSource[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupSubtitleText],
                TextSource[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupHeaderText],
                TextSource[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupButtonText]));
        });
    }
    
    public IMvxAsyncCommand ItemClickedCommand { get; set; }
}