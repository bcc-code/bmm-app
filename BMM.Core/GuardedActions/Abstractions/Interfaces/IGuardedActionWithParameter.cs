using System.Threading.Tasks;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Abstractions.Interfaces
{
    public interface IGuardedActionWithParameter<TParameter> : IBaseGuardedAction
    {
        IMvxAsyncCommand<TParameter> Command { get; }
        Task GuardedExecute(TParameter parameter);
    }
}