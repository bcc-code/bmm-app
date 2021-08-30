using System.Threading.Tasks;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Base.Interfaces
{
    public interface IGuardedActionWithParameter<TParameter> : IBaseGuardedAction
    {
        IMvxAsyncCommand<TParameter> Command { get; }
        Task ExecuteGuarded(TParameter parameter);
    }
}