using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.Settings.Interfaces
{
    public interface IChangeNotificationSettingStateAction : IGuardedActionWithParameter<bool>
    {
    }
}