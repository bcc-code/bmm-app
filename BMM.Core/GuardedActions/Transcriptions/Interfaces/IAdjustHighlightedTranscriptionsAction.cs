using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Transcriptions.Interfaces;

public interface IAdjustHighlightedTranscriptionsAction
    : IGuardedActionWithParameter<long>,
      IDataContextGuardedAction<ReadTranscriptionViewModel>
{
}