using BMM.Core.Constants;
using BMM.Core.GuardedActions.Abstractions.Interfaces;

namespace BMM.Core.GuardedActions.GuardedActionExtensions
{
    public static class GuardedActionsExtensions
    {
        public static void AttachDataContext<TDataContext>(
            this IDataContextGuardedAction<TDataContext> guardedAction,
            TDataContext dataContext)
        {
            guardedAction.Attach(ExecutionContextKeys.DataContext, dataContext);
        }

        public static TDataContext GetDataContext<TDataContext>(this IDataContextGuardedAction<TDataContext> guardedAction)
        {
            return guardedAction.Get<TDataContext>(ExecutionContextKeys.DataContext);
        }
    }
}