using System.Threading.Tasks;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Abstractions.Interfaces
{
    public interface IGuardedAction : IBaseGuardedAction
    {
        IMvxAsyncCommand Command { get; }
        Task ExecuteGuarded();
    }
}