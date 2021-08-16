using BMM.Core.ExceptionHandlers.Interfaces;

namespace BMM.Core.GuardedActions.Base.Interfaces
{
    public interface IBaseGuardedAction
    {
        void Attach<T>(string key, T value);
        T Get<T>(string key);
        IGuardInvoker Invoker { get; set; }
        IGenericActionExceptionHandler GenericExceptionHandler { get; set; }
    }
}