using BMM.Core.GuardedActions.Documents;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.GuardedActions.Test;
using BMM.Core.Test.Unit.GuardedActions.Base;
using MvvmCross.Plugin.Messenger;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions
{
    [TestFixture]
    public class TestActionTests : GuardedActionsTestBase<ILoadMoreDocumentsAction>
    {
        private IMvxMessenger _mvxMessenger;

        public override void SetUp()
        {
            base.SetUp();
            _mvxMessenger = Substitute.For<IMvxMessenger>();
        }

        protected override ILoadMoreDocumentsAction CreateAction()
        {
            return new LoadMoreDocumentsAction(_mvxMessenger);
        }

        [Test]
        public void Asdadas()
        {
            GuardedAction.ExecuteGuarded();
        }
    }
}