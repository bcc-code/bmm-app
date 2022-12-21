using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Documents.Interfaces
{
    public interface IPrepareTileCarouselItemsAction
        : IGuardedActionWithParameterAndResult<IList<Document>, IList<IDocumentPO>>,
          IDataContextGuardedAction<ExploreNewestViewModel>
    {
    }
}