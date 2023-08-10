using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs;

namespace BMM.Core.GuardedActions.App.Interfaces;

public interface IGetAvailableAppIconsAction : IGuardedActionWithResult<IList<AppIconPO>>
{
}