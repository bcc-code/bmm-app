using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Documents.Interfaces;

namespace BMM.Core.GuardedActions.Documents
{
    public class PrepareCoversCarouselItemsAction
        : GuardedActionWithParameterAndResult<IList<Document>, IList<Document>>,
          IPrepareCoversCarouselItemsAction
    {
        protected override async Task<IList<Document>> Execute(IList<Document> docs)
        {
            await Task.CompletedTask;
            var adjustedList = docs.ToList();

            var carouselHeaders = adjustedList
                .OfType<DiscoverSectionHeader>()
                .Where(d => d.UseCoverCarousel)
                .ToList();

            foreach (var carouselHeader in carouselHeaders)
            {
                var coverDocuments = new List<CoverDocument>();
                int currentIndex = adjustedList.IndexOf(carouselHeader) + 1;

                while (true)
                {
                    if (currentIndex >= adjustedList.Count)
                        break;

                    if (!(adjustedList[currentIndex] is CoverDocument element))
                        break;

                    coverDocuments.Add(element);
                    adjustedList.RemoveAt(currentIndex);
                }

                if (coverDocuments.Any())
                    adjustedList.Insert(currentIndex, new CoverCarouselCollection(new ObservableCollection<Document>(coverDocuments)));
            }

            return adjustedList;
        }
    }
}