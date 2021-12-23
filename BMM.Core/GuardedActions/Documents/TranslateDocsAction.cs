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
                HandleDiscoverSection(document);
                HandleInfoMessage(document);
            }

            return adjustedDocs;
        }

        private void HandleDiscoverSection(Document document)
        {
            if (!(document is DiscoverSectionHeader sectionHeader))
                return;

            sectionHeader.Title = _bmmLanguageBinder.GetTranslationsSafe(sectionHeader.GetTranslationKey(), sectionHeader.Title);
        }

        private void HandleInfoMessage(Document document)
        {
            if (!(document is InfoMessage infoMessage))
                return;

            infoMessage.MessageText = _bmmLanguageBinder.GetTranslationsSafe(infoMessage.GetTranslationKey(), infoMessage.TranslatedMessage);
        }
    }
}