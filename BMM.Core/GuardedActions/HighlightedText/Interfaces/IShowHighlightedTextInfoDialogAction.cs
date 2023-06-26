using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.HighlightedText.Interfaces;

public interface IShowHighlightedTextInfoDialogAction 
    : IGuardedAction,
      IDataContextGuardedAction<HighlightedTextTrackViewModel>
{
}