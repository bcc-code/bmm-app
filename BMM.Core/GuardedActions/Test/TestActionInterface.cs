using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Test.Interfaces;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.GuardedActions.Test
{
    public class TestActionInterface : GuardedAction, ITestActionInterface
    {
        private readonly IMvxMessenger _mvxMessenger;

        public TestActionInterface(IMvxMessenger mvxMessenger)
        {
            _mvxMessenger = mvxMessenger;
        }

        protected override async Task Execute()
        {
            _mvxMessenger.Publish(new BackgroundTaskMessage(this, () => Task.CompletedTask));
        }
    }
}