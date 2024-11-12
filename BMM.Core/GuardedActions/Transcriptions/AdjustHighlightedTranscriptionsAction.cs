using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Transcriptions.Interfaces;
using BMM.Core.Models.POs.SuggestEdit.Interfaces;
using BMM.Core.Models.POs.Transcriptions;
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
        var transcriptionItems = DataContext
            .Transcriptions
            .OfType<ReadTranscriptionsPO>()
            .ToList();
        
        var currentItem = transcriptionItems
            .FirstOrDefault(t => t.Transcription.Start < positionInSeconds && t.Transcription.End > positionInSeconds);

        if (currentItem == null)
        {
            if (!DataContext.HasTimeframes)
                transcriptionItems.ForEach(x => x.IsHighlighted = true);
            
            return Task.CompletedTask;
        }

        int indexOfCurrentItem = transcriptionItems
            .IndexOf(currentItem);

        for (int i = 0; i < transcriptionItems.Count; i++)
            transcriptionItems[i].IsHighlighted = i <= indexOfCurrentItem;
        
        DataContext.AdjustScrollPositionInteraction.Raise(indexOfCurrentItem);
        return Task.CompletedTask;
    }
}