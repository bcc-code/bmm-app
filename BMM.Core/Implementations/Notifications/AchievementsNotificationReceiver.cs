using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Implementations.Notifications.Data;

namespace BMM.Core.Implementations.Notifications
{
    public class AchievementsNotificationReceiver : IReceive<AchievementNotification>
    {
        private readonly ICheckAndShowAchievementUnlockedScreenAction _checkAndShowAchievementUnlockedScreenAction;

        public AchievementsNotificationReceiver(ICheckAndShowAchievementUnlockedScreenAction checkAndShowAchievementUnlockedScreenAction)
        {
            _checkAndShowAchievementUnlockedScreenAction = checkAndShowAchievementUnlockedScreenAction;
        }

        public void UserClickedRemoteNotification(AchievementNotification notification)
        {
        }

        public async void OnNotificationReceived(AchievementNotification notification)
        {
            await _checkAndShowAchievementUnlockedScreenAction.ExecuteGuarded();
        }
    }
}