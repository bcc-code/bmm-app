using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.ExceptionHandlers.Interfaces.Base;
using BMM.Core.GuardedActions.Base.Interfaces;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Base
{
    public abstract class GuardedActionsTestBase<TAction>
        where TAction : class, IGuardedAction
    {
        protected TAction GuardedAction { get; private set; }
        protected IMvxMessenger MessengerMock { get; private set; }
        protected IMvxNavigationService NavigationServiceMock { get; private set; }
        protected IGuardInvoker GuardInvoker { get; private set; }

        protected abstract TAction CreateAction();

        [SetUp]
        public virtual void SetUp()
        {
            MessengerMock = Substitute.For<IMvxMessenger>();
            NavigationServiceMock = Substitute.For<IMvxNavigationService>();
            GuardInvoker = Substitute.For<IGuardInvoker>();

            GuardedAction = CreateAction();

            GuardedAction.Invoker = GuardInvoker;

            GuardInvoker.Invoke(
                Arg.Any<Func<Task>>(),
                Arg.Any<Func<Exception, Task>>(),
                Arg.Any<Func<Task>>(),
                Arg.Any<IEnumerable<IActionExceptionHandler>>()
                )
                .Returns(async (args) =>
                {
                    bool isFinallyActionAvailable = args[2] != null;
                    var job = (Func<Task>)args[0];
                    var onFinally = (Func<Task>)args[2];

                    if (isFinallyActionAvailable)
                        await Task.WhenAll(job(), onFinally());
                    else
                        await job();
                });
        }

        [TearDown]
        public virtual void TearDown()
        {
        }
    }
}