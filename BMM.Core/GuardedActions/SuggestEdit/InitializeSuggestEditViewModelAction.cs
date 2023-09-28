using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.SuggestEdit.Interfaces;
using BMM.Core.Models.POs.SuggestEdit;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.SuggestEdit;

public class InitializeSuggestEditViewModelAction
    : GuardedAction,
      IInitializeSuggestEditViewModelAction
{
    private readonly ITranscriptionClient _transcriptionClient;

    public InitializeSuggestEditViewModelAction(ITranscriptionClient transcriptionClient)
    {
        _transcriptionClient = transcriptionClient;
    }
    
    private SuggestEditViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute()
    {
        var po = DataContext.NavigationParameter;
        var transcription = await _transcriptionClient.GetTranscription(
            po.TrackPO.Id,
            SuggestEditViewModel.DefaultTranscriptionLanguage,
            po.SearchHighlight.FirstSegmentIndex,
            po.SearchHighlight.LastSegmentIndex);

        DataContext.Transcriptions.AddRange(transcription.Select(t => new TranscriptionPO(t, DataContext.EditedAction)));
    }
}