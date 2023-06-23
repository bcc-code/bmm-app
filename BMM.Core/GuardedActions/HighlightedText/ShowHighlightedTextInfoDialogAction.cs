using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.HighlightedText.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.Parameters;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Base;

namespace BMM.Core.GuardedActions.HighlightedText;

public class ShowHighlightedTextInfoDialogAction : GuardedAction, IShowHighlightedTextInfoDialogAction
{
    private readonly IDialogService _dialogService;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

    public ShowHighlightedTextInfoDialogAction(
        IDialogService dialogService,
        IBMMLanguageBinder bmmLanguageBinder,
        IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
    {
        _dialogService = dialogService;
        _bmmLanguageBinder = bmmLanguageBinder;
        _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
    }

    private HighlightedTextTrackViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute()
    {
        string header = DataContext.IsSong
            ? _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_FromSongTreasuresPopupHeaderText]
            : _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupHeaderText];
        
        string title = DataContext.IsSong
            ? _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_FromSongTreasuresPopupTitleText]
            : _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupTitleText];
            
        string subtitle = DataContext.IsSong
            ? _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_FromSongTreasuresPopupSubtitleText]
            : _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupSubtitleText];
        
        await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
        {
            return _dialogService.ShowDialog(new DialogParameter(
                title,
                subtitle,
                header,
                _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupButtonText]));
        });
    }
}