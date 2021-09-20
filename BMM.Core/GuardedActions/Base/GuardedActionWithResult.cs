using System;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Base
{
    public abstract class GuardedActionWithResult<TResult>
        : BaseGuardedAction,
          IGuardedActionWithResult<TResult>
    {
        private IMvxAsyncCommand _command;
        public IMvxAsyncCommand Command => _command ??= new MvxAsyncCommand(ExecuteGuarded, GuardedCanExecute);

        protected abstract Task<TResult> Execute();
        protected virtual bool CanExecute() => true;

        protected virtual Task OnException(Exception ex) => Task.CompletedTask;
        protected virtual Task OnFinally() => Task.CompletedTask;

        public async Task<TResult> ExecuteGuarded() => await Invoker.Invoke(Execute, OnException, OnFinally, ExceptionHandlers());

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