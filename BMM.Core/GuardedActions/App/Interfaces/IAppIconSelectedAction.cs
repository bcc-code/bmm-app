using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.Enums;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.App.Interfaces;

public interface IAppIconSelectedAction
    : IGuardedActionWithParameter<AppIconType>,
      IDataContextGuardedAction<IAppIconViewModel>
{
}