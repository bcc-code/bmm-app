using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class BrowseViewModel : DocumentsViewModel
    {
        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var browseItems = await Client.Browse.Get();

            var browseItemsList = browseItems?.ToList();
            PrepareCoversCarouselItems(browseItemsList);

            return browseItemsList;
        }

        private static void PrepareCoversCarouselItems(IList<Document> filteredDocs)
        {
            if (filteredDocs == null)
                return;

            var carouselHeaders = filteredDocs
                .OfType<DiscoverSectionHeader>()
                .Where(d => d.UseCoverCarousel)
                .ToList();

            if (carouselHeaders.Any())
                carouselHeaders.First().IsSeparatorVisible = false;

            foreach (var carouselHeader in carouselHeaders)
            {
                var coverDocuments = new List<CoverDocument>();
                int currentIndex = filteredDocs.IndexOf(carouselHeader) + 1;

                while (true)
                {
                    if (currentIndex >= filteredDocs.Count)
                        break;

                    if (!(filteredDocs[currentIndex] is CoverDocument element))
                        break;

                    coverDocuments.Add(element);
                    filteredDocs.RemoveAt(currentIndex);
                }

                if (coverDocuments.Any())
                    filteredDocs.Insert(currentIndex, new CoverCarouselCollection(new ObservableCollection<Document>(coverDocuments)));
            }
        }
    }
}