using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.Documents.Interfaces
{
    public interface ITranslateDocsAction : IGuardedActionWithParameterAndResult<IList<Document>, IList<Document>>
    {
    }
}