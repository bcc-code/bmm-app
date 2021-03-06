using System;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.Base
{
    public abstract class GuardedActionWithParameter<TParameter>
        : BaseGuardedAction,
          IGuardedActionWithParameter<TParameter>
    {
        private IMvxAsyncCommand<TParameter> _command;
        public IMvxAsyncCommand<TParameter> Command => _command ??= new MvxAsyncCommand<TParameter>(ExecuteGuarded, GuardedCanExecute);

        protected abstract Task Execute(TParameter parameter);
        protected virtual bool CanExecute(TParameter parameter) => true;
        protected virtual Task OnException(Exception ex, TParameter parameter) => Task.CompletedTask;
        protected virtual Task OnFinally(TParameter parameter) => Task.CompletedTask;

        public async Task ExecuteGuarded(TParameter parameter)
            => await Invoker.Invoke(
                () => Execute(parameter),
                ex => OnException(ex, parameter),
                () => OnFinally(parameter),
                ExceptionHandlers());

        private bool GuardedCanExecute(TParameter parameter)
        {
            try
            {
                return CanExecute(parameter);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }
    }
}