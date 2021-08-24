using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.GuardedActions.Documents.Interfaces
{
    public interface ILoadMoreDocumentsAction
        : IGuardedAction,
          IDataContextGuardedAction<ILoadMoreDocumentsViewModel>
    {
    }
}