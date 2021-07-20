using System.Threading.Tasks;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Abstractions.Interfaces
{
    public interface IGuardedActionWithParameterAndResult<TParameter, TResult> : IBaseGuardedAction
    {
        IMvxAsyncCommand<TParameter> Command { get; }
        Task<TResult> ExecuteGuarded(TParameter parameter);
    }
}