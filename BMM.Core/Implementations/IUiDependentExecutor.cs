using System;

namespace BMM.Core.Implementations
{
    public interface IUiDependentExecutor
    {
        void ExecuteWhenReady(Action action);
    }
}
