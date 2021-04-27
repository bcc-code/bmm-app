using System;

namespace BMM.Core.Implementations
{
    public class NullDependentExecutor : IUiDependentExecutor
    {
        public void ExecuteWhenReady(Action action)
        {
            action.Invoke();
        }
    }
}