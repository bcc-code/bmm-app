using System.Threading.Tasks;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Base.Interfaces
{
    public interface IGuardedActionWithResult<TResult> : IBaseGuardedAction
    {
        IMvxAsyncCommand Command { get; }
        Task<TResult> ExecuteGuarded();
    }
}