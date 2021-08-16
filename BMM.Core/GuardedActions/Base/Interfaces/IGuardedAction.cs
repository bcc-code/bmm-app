using System.Threading.Tasks;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Base.Interfaces
{
    public interface IGuardedAction : IBaseGuardedAction
    {
        IMvxAsyncCommand Command { get; }
        Task ExecuteGuarded();
    }
}