using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.ExceptionHandlers.Interfaces.Base;
using BMM.Core.GuardedActions.Base.Interfaces;
using NSubstitute;

namespace BMM.Core.Test.Unit.GuardedActions.Base
{
    public abstract class GuardedActionWithParameterAndResultTests<TAction, TParameter, TResult> : GuardInvokerTestsBase<TAction>
        where TAction : class, IGuardedActionWithParameterAndResult<TParameter, TResult>
    {
        public override void SetUp()
        {
            base.SetUp();

            GuardedAction.Invoker = GuardInvokerMock;

            GuardInvokerMock.Invoke(
                    Arg.Any<Func<Task<TResult>>>(),
                    Arg.Any<Func<Exception, Task>>(),
                    Arg.Any<Func<Task>>(),
                    Arg.Any<IEnumerable<IActionExceptionHandler>>())
                .Returns(async args =>
                {
                    var job = (Func<Task<TResult>>)args[0];
                    var onFinally = (Func<Task>)args[2];

                    try
                    {
                        return await job();
                    }
                    finally
                    {
                        await onFinally();
                    }
                });
        }
    }
}