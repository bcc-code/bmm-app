using BMM.Core.GuardedActions.Test;
using BMM.Core.GuardedActions.Test.Interfaces;
using BMM.Core.Test.Unit.GuardedActions.Base;
using MvvmCross.Plugin.Messenger;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions
{
    [TestFixture]
    public class TestActionTests : GuardedActionsTestBase<ITestActionInterface>
    {
        private IMvxMessenger _mvxMessenger;

        public override void SetUp()
        {
            base.SetUp();
            _mvxMessenger = Substitute.For<IMvxMessenger>();
        }

        protected override ITestActionInterface CreateAction()
        {
            return new TestActionInterface(_mvxMessenger);
        }

        [Test]
        public void Asdadas()
        {
            GuardedAction.ExecuteGuarded();
        }
    }
}