using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.HighlightedText.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.Parameters;
using BMM.Core.Translation;
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

    protected override async Task Execute()
    {
        await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
        {
            return _dialogService.ShowDialog(new DialogParameter(
                _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupTitleText],
                _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupSubtitleText],
                _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupHeaderText],
                _bmmLanguageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribedPopupButtonText]));
        });
    }
}