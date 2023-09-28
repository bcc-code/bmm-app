using BMM.Core.Extensions;
using BMM.Core.GuardedActions.SuggestEdit.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Models.POs.SuggestEdit;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels;

public class SuggestEditViewModel : BaseViewModel<HighlightedTextPO>
{
    public const string DefaultTranscriptionLanguage = "nb";
    private readonly IInitializeSuggestEditViewModelAction _initializeSuggestEditViewModelAction;
    private readonly ISubmitSuggestedEditAction _submitSuggestedEditAction;

    public SuggestEditViewModel(
        IInitializeSuggestEditViewModelAction initializeSuggestEditViewModelAction,
        ISubmitSuggestedEditAction submitSuggestedEditAction)
    {
        _initializeSuggestEditViewModelAction = initializeSuggestEditViewModelAction;
        _submitSuggestedEditAction = submitSuggestedEditAction;
        _initializeSuggestEditViewModelAction.AttachDataContext(this);
        _submitSuggestedEditAction.AttachDataContext(this);
    }

    public BmmObservableCollection<TranscriptionPO> Transcriptions { get; } = new();
    public IMvxAsyncCommand SubmitCommand  => _submitSuggestedEditAction.Command;
    public bool CanSubmit => SubmitCommand.CanExecute();
    
    public override async Task Initialize()
    {
        await _initializeSuggestEditViewModelAction.ExecuteGuarded();
    }

    public void EditedAction()
    {
        SubmitCommand.RaiseCanExecuteChanged();
        RaisePropertyChanged(nameof(CanSubmit));
    }
}