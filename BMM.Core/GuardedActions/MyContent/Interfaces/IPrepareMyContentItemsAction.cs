using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.GuardedActions.MyContent.Interfaces;

public interface IPrepareMyContentItemsAction
    : IGuardedActionWithParameterAndResult<IEnumerable<IDocumentPO>, IList<IDocumentPO>>
{
}