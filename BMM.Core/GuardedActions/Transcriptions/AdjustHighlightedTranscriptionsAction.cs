using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Transcriptions.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Transcriptions;

public class AdjustHighlightedTranscriptionsAction
    : GuardedActionWithParameter<long>,
      IAdjustHighlightedTranscriptionsAction
{
    private ReadTranscriptionViewModel DataContext => this.GetDataContext();
    
    protected override Task Execute(long currentPosition)
    {
        long positionInSeconds = (long)TimeSpan.FromMilliseconds(currentPosition).TotalSeconds;
        
        var currentItem = DataContext
            .Transcriptions
            .FirstOrDefault(t => t.Transcription.Start < positionInSeconds && t.Transcription.End > positionInSeconds);

        if (currentItem == null)
        {
            if (DataContext.Transcriptions.Any() && DataContext.Transcriptions.Last().Transcription.End == 0)
                DataContext.Transcriptions.All(x => x.IsHighlighted = true);
            return Task.CompletedTask;
        }

        int indexOfCurrentItem = DataContext
            .Transcriptions
            .IndexOf(currentItem);

        for (int i = 0; i < DataContext.Transcriptions.Count; i++)
            DataContext.Transcriptions[i].IsHighlighted = i <= indexOfCurrentItem;
        
        DataContext.AdjustScrollPositionInteraction.Raise(indexOfCurrentItem);
        return Task.CompletedTask;
    }
}