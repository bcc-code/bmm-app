using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.TrackCollections;

namespace BMM.Core.GuardedActions.MyContent.Interfaces;

public interface IPrepareDownloadedContentItemsAction
    : IGuardedActionWithParameterAndResult<IEnumerable<TrackCollectionPO>, IList<DocumentPO>>
{
}