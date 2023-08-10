using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Implementations.Notifications.Data;

namespace BMM.Core.Implementations.Notifications
{
    public class AchievementsNotificationReceiver : IReceive<AchievementNotification>
    {
        private readonly IShowAchievementUnlockedScreenAction _showAchievementUnlockedScreenAction;

        public AchievementsNotificationReceiver(IShowAchievementUnlockedScreenAction showAchievementUnlockedScreenAction)
        {
            _showAchievementUnlockedScreenAction = showAchievementUnlockedScreenAction;
        }

        public void UserClickedRemoteNotification(AchievementNotification notification)
        {
        }

        public async void OnNotificationReceived(AchievementNotification notification)
        {
            await _showAchievementUnlockedScreenAction.ExecuteGuarded();
        }
    }
}