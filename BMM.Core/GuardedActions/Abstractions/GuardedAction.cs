using System;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Abstractions.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Abstractions
{
    public abstract class GuardedAction
        : BaseGuardedAction,
          IGuardedAction
    {
        private IMvxAsyncCommand _command;
        public IMvxAsyncCommand Command => _command ??= new MvxAsyncCommand(ExecuteGuarded, GuardedCanExecute);

        protected abstract Task Execute();
        protected virtual bool CanExecute() => true;
        protected virtual Task OnException(Exception exception) => Task.CompletedTask;
        protected virtual Task OnFinally() => Task.CompletedTask;

        public async Task ExecuteGuarded() => await Invoker.Invoke(Execute, OnException, OnFinally, ExceptionHandlers());

        private bool GuardedCanExecute()
        {
            try
            {
                return CanExecute();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }
    }
}