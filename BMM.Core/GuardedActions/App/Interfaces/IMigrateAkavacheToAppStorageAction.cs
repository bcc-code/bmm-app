using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.App.Interfaces
{
    public interface IMigrateAkavacheToAppStorageAction : IGuardedActionWithResult<bool>
    {
    }
}