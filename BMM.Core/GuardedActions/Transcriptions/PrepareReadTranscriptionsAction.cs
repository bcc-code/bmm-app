using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Transcriptions.Interfaces;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Transcriptions;

public class PrepareReadTranscriptionsAction
    : GuardedActionWithParameter<int>,
      IPrepareReadTranscriptionsAction
{
    private readonly ITracksClient _tracksClient;

    public PrepareReadTranscriptionsAction(ITracksClient tracksClient)
    {
        _tracksClient = tracksClient;
    }
    
    private ReadTranscriptionViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute(int trackId)
    {
        var response = await _tracksClient.GetTranscriptions(trackId);
        DataContext.Transcriptions.ReplaceWith(response.Select(t => new ReadTranscriptionsPO(t)).ToList());
    }
}