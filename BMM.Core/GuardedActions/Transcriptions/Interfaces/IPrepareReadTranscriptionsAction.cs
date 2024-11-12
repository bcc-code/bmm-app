using BMM.Api.Abstraction;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Transcriptions.Interfaces;

public interface IPrepareReadTranscriptionsAction
    : IGuardedActionWithParameter<ITrackModel>,
      IDataContextGuardedAction<ReadTranscriptionViewModel>
{
}