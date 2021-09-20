using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.ExceptionHandlers.Interfaces.Base;
using BMM.Core.GuardedActions.Base.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Base
{
    public abstract class GuardedActionWithParameterTestBase<TAction, TParameter> : GuardInvokerTestsBase<TAction>
        where TAction : class, IGuardedActionWithParameter<TParameter>
    {
        public override void SetUp()
        {
            base.SetUp();
            GuardedAction.Invoker = GuardInvokerMock;

            GuardInvokerMock.Invoke(
                    Arg.Any<Func<Task>>(),
                    Arg.Any<Func<Exception, Task>>(),
                    Arg.Any<Func<Task>>(),
                    Arg.Any<IEnumerable<IActionExceptionHandler>>())
                .Returns(async (args) =>
                {
                    var job = (Func<Task>)args[0];
                    var onFinally = (Func<Task>)args[2];

                    try
                    {
                        await job();
                    }
                    finally
                    {
                        await onFinally();
                    }
                });
        }
    }
}