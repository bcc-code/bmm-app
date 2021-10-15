using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;

namespace BMM.Core.GuardedActions.Documents
{
    public class TranslateDocsAction
        : GuardedActionWithParameterAndResult<IList<Document>, IList<Document>>,
          ITranslateDocsAction
    {
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public TranslateDocsAction(IBMMLanguageBinder bmmLanguageBinder)
        {
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        protected override async Task<IList<Document>> Execute(IList<Document> docs)
        {
            await Task.CompletedTask;
            var adjustedDocs = docs.ToList();

            foreach (var document in adjustedDocs)
            {
                if (!(document is DiscoverSectionHeader sectionHeader))
                    continue;

                sectionHeader.Title = _bmmLanguageBinder.GetTranslationsSafe(sectionHeader.GetTranslationKey(), sectionHeader.Title);
            }

            return adjustedDocs;
        }
    }
}