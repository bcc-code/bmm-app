using BMM.Core.GuardedActions.Base.Interfaces;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Base
{
    [TestFixture]
    public abstract class GuardInvokerTestsBase<TAction>
    {
        protected TAction GuardedAction { get; private set; }

        protected IMvxMessenger MessengerMock { get; private set; }

        protected IMvxNavigationService NavigationServiceMock { get; private set; }

        protected IGuardInvoker GuardInvokerMock { get; private set; }

        protected abstract TAction CreateAction();

        [SetUp]
        public virtual void SetUp()
        {
            PrepareMocks();
            GuardedAction = CreateAction();
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        protected virtual void PrepareMocks()
        {
            MessengerMock = Substitute.For<IMvxMessenger>();
            NavigationServiceMock = Substitute.For<IMvxNavigationService>();
            GuardInvokerMock = Substitute.For<IGuardInvoker>();
        }
    }
}