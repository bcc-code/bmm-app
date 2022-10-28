using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.YearInReview.Interfaces
{
    public interface IInitializeYearInReviewViewModelAction
        : IGuardedAction,
          IDataContextGuardedAction<IYearInReviewViewModel>
    {
    }
}