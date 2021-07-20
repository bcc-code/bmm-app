namespace BMM.Core.GuardedActions.Abstractions.Interfaces
{
    public interface IBaseGuardedAction
    {
        void Attach<T>(string key, T value);
        T Get<T>(string key);
        IGuardInvoker Invoker { get; set; }
    }
}